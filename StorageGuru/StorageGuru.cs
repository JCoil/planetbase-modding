using Planetbase;
using UnityEngine;
using Redirection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace StorageManager
{
    public class StorageGuru : IMod
    {
        private static StorageGuru _instance;
        public static StorageGuru GetInstance() { return _instance; }
        public static string STORAGE_MANIFEST_PATH = @"Mods\Settings\storage_manifest.txt";

        public static StorageController Controller { get; private set; }

        private static GameState gameState;
        private static GameStateGame game;
        private static Module activeModule;
        private static GuiMenuSystem menuSystem;
        private static GuiMenu activeMenu;
        public static Dictionary<ResourceType, GuiDefinitions.Callback> allResources { get; private set; }
        public static Type[] allResourceTypes { get; private set; }
        public static Dictionary<ResourceType, IconPair> allIcons { get; private set; }


        bool initialising = true;
        bool menuSetUp = false;
        private bool enableAll = true;

        public void Init()
        {
            _instance = this;

            STORAGE_MANIFEST_PATH = Path.Combine(Util.getFilesFolder(), STORAGE_MANIFEST_PATH);

            Controller = new StorageController();
            allResources = StorageMapping.GetAllResources();
            allResourceTypes = allResources.Select(x => x.Key.GetType()).ToArray();
            allIcons = ContentManager.LoadContent(allResources.Keys.ToArray());

            Redirector.PerformRedirections();
            UnityEngine.Debug.Log("[MOD] StorageManager activated");
        }

        public void Update()
        {
            gameState = GameManager.getInstance().getGameState();

            if (gameState is GameStateTitle)
            {
                if (!initialising)
                {
                    Controller.SaveManifest(STORAGE_MANIFEST_PATH);
                    FirstUpdate();
                }
            }
            else if(gameState is GameStateGame)
            {
                if(initialising)
                {
                    FirstUpdate();
                    initialising = false;
                }
                GameUpdate();
                Controller.ConsolidateDefinitions();
            }

        }

        private void GameUpdate()
        {
            // Check if we're editing a storage module
            game = (GameStateGame)gameState;
            activeModule = game.mActiveModule;
            menuSystem = game.mMenuSystem;
            activeMenu = menuSystem.getCurrentMenu();

            if (activeModule != null && activeModule.getCategory() == Module.Category.Storage)
            {
                if (activeMenu == menuSystem.mMenuEdit && !menuSetUp)
                {
                    SetupEditMenu(activeModule);
                    menuSetUp = true;
                }
            }
            else
            {
                menuSetUp = false;
            }

            RedirectCharacters();
        }

        public void FirstUpdate()
        {
            Controller = new StorageController();
            Controller.LoadManifest(STORAGE_MANIFEST_PATH);
            initialising = false;
        }

        private void SetupEditMenu(Module module)
        {
            var menu = menuSystem.mMenuEdit;
            var definitions = Controller.GetDefinition(module);

            var backItem = menu.getBackItem();
            menu.clear();

            foreach (var resource in allResources)
            {
                Texture2D icon; 
                string tooltip = resource.Key.getName().Trim();

                if (definitions.Count == 0 || !definitions.Contains(resource.Key.GetType()))
                {
                    icon = allIcons[resource.Key].disabledIcon;
                    tooltip += " - OFF";
                }
                else
                {
                    icon = allIcons[resource.Key].enabledIcon;
                    tooltip += " - ON";
                }

                menu.addItem(new GuiMenuItem(icon, tooltip, resource.Value));
            }

            enableAll = definitions.Count == allResources.Count ? false : (definitions.Count == 0 ? true : enableAll);

            if(enableAll)
            {
                menu.addItem(new GuiMenuItem(ContentManager.StorageEnableIcon, "Enable All", EnableAllCallback));
            }
            else
            {
                menu.addItem(new GuiMenuItem(ContentManager.StorageDisableIcon, "Disable All", DisableAllCallback));
            }

            menu.addItem(backItem);
            menuSetUp = true;
        }

        public void StorageCallback(Type resource)
        {
            if (resource != null)
            {
                Controller.ToggleDefinitions(activeModule, new Type[] { resource });
                SetupEditMenu(activeModule);
            }
        }

        public void DisableAllCallback(object parameter)
        {
            Controller.RemoveDefinitions(activeModule, allResourceTypes);
            SetupEditMenu(activeModule);
        }

        public void EnableAllCallback(object parameter)
        {
            Controller.AddDefinitions(activeModule, allResourceTypes);
            SetupEditMenu(activeModule);
        }

        private Dictionary<Character, Resource> carriedResources;
        private Dictionary<Character, Resource> newCarriedResources;
        private List<Type> uniqueCarriedResources;
        private Dictionary<Type, List<Module>> resourceTargets;
        private Dictionary<Character, Module> newCharacterTargets;

        private Dictionary<Character, Module> characterTargets = new Dictionary<Character, Module>();

        private void RedirectCharacters()
        {

            carriedResources = Character.mCharacters.Where(x => x.getLoadedResource() != null).ToDictionary(y => y, x => x.getLoadedResource()); // Get all carried resources
            newCarriedResources = carriedResources.Where(x => !characterTargets.ContainsKey(x.Key)).ToDictionary(y => y.Key, x => x.Value); // That haven't been processed

            if(newCarriedResources.Count == 0) { return; }

            newCarriedResources = newCarriedResources.Where(x => x.Key.getTarget() != null // That are valid storage
            && x.Key.getTarget().getSelectable() != null
            && x.Key.getTarget().getSelectable() is Module
            && ((Module)x.Key.getTarget().getSelectable()).isBuilt()
            && ((Module)x.Key.getTarget().getSelectable()).getCategory() == Module.Category.Storage).ToDictionary(x => x.Key, y => y.Value);

            uniqueCarriedResources = newCarriedResources.Select(x => x.Value.getResourceType().GetType()).Distinct().ToList(); // Get unique carried resource types

            resourceTargets = uniqueCarriedResources.ToDictionary(x => x, y => Controller.GetValidModules(y)); // Get possible targets for resource types

            newCharacterTargets = newCarriedResources.ToDictionary(x => x.Key, y => Controller.FindNearestModule(y.Key.getPosition(), // Find nearest
                resourceTargets[y.Value.getResourceType().GetType()]));

            characterTargets = characterTargets.Where(x => newCarriedResources.ContainsKey(x.Key)).ToDictionary(x => x.Key, y => y.Value); // Removed completed charcaters from list

            foreach(var kvp in newCharacterTargets)
            {
                if(kvp.Value != null)
                {
                    kvp.Key.setTarget(new Target(kvp.Value, kvp.Value.getRadius() / 1.8f));
                }

                characterTargets.Add(kvp.Key, kvp.Value);
            }
        }

        //[Obsolete]
        //private void RedirectCharactersAlt()
        //{
        //    Stopwatch timer = Stopwatch.StartNew();

        //    // Get all characters taking resources to built storage modules and redirect them to valid module
        //    foreach (var c in Character.mCharacters.Where(x => x.getLoadedResource() != null))
        //    {
        //        var currentTarget = c.getTarget();

        //        if (currentTarget != null)
        //        {
        //            var targetModule = currentTarget.getSelectable() as Module;

        //            if (targetModule != null && targetModule.isBuilt() && targetModule.getCategory() == Module.Category.Storage)
        //            {
        //                var newTarget = Controller.FindNearestModule(c.getPosition(), Controller.GetValidModules(c.getLoadedResource().getResourceType().GetType()));
        //                if (newTarget != null)
        //                {
        //                    // Target middle ring of storage
        //                    var t = new Target(newTarget, newTarget.getRadius() / 1.8f);
        //                    c.setTarget(t);
        //                }
        //            }
        //        }
        //    }

        //    timer.Stop();
        //    avg -= avg / 20;
        //    avg += timer.Elapsed.TotalMilliseconds / 20;
        //    UnityEngine.Debug.Log(avg.ToString("0.000"));
        //}

        public static void RefreshStorage(Module module, HashSet<Type> allowedResources)
        {
            if (module == null) { return; }

            if (module.mResourceStorage != null && module.mResourceStorage.mSlots != null)
            {
                foreach (var slot in module.mResourceStorage.mSlots)
                {
                    if (slot != null && slot.mResources != null)
                    {
                        foreach (var res in slot.mResources)
                        {
                            if (allowedResources.Contains(res.getResourceType().GetType()))
                            {
                                res.setState(Resource.State.Stored);
                            }
                            else if (Controller.IsStorageAvailable(res.getResourceType().GetType()))
                            {
                                res.setState(Resource.State.Idle);
                            }
                        }
                    }
                }
            }
        }
    }

    public abstract class CustomStorage : Module
    {
        [RedirectFrom(typeof(Module))]
        public override bool isEditable()
        {
            return StorageMapping.isEditable(getModuleType());
        }
    }

    public struct IconPair
    {
        public Texture2D enabledIcon;
        public Texture2D disabledIcon;

        public IconPair(Texture2D enabled, Texture2D disabled)
        {
            enabledIcon = enabled;
            disabledIcon = disabled;
        }
    }

}
