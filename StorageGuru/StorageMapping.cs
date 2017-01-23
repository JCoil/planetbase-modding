using Planetbase;
using System.Collections.Generic;

namespace StorageManager
{
    internal static class StorageMapping
    {
        internal static bool isEditable(ModuleType type)
        {
            if (type is ModuleTypeAirlock){ return false;}
            if (type is ModuleTypeAntiMeteorLaser){ return false;}
            if (type is ModuleTypeBar){ return true;}
            if (type is ModuleTypeBasePad){ return false;}
            if (type is ModuleTypeBioDome){ return true;}
            if (type is ModuleTypeCabin){ return true; }
            if (type is ModuleTypeCanteen){ return true; }
            if (type is ModuleTypeControlCenter){ return true; }
            if (type is ModuleTypeDorm){ return true; }
            if (type is ModuleTypeFactory){ return true; }
            if (type is ModuleTypeLab){ return true; }
            if (type is ModuleTypeLandingPad){ return false;}
            if (type is ModuleTypeLightningRod){ return false;}
            if (type is ModuleTypeMine){ return false;}
            if (type is ModuleTypeMonolith){ return false;}
            if (type is ModuleTypeMultiDome){ return true; }
            if (type is ModuleTypeOxygenGenerator){ return false;}
            if (type is ModuleTypePowerCollector){ return false;}
            if (type is ModuleTypeProcessingPlant){ return true; }
            if (type is ModuleTypePyramid){ return false;}
            if (type is ModuleTypeRadioAntenna){ return false;}
            if (type is ModuleTypeRoboticsFacility){ return true; }
            if (type is ModuleTypeSickBay){ return true; }
            if (type is ModuleTypeSignpost){ return false;}
            if (type is ModuleTypeSolarPanel){ return false;}
            if (type is ModuleTypeStarport){ return false;}
            if (type is ModuleTypeStorage){ return true; }
            if (type is ModuleTypeTelescope){ return false;}
            if (type is ModuleTypeWaterExtractor){ return false;}
            if (type is ModuleTypeWaterTank){ return false;}
            if (type is ModuleTypeWindTurbine){ return false;}
            return false;
        }

        internal static Dictionary<ResourceType, GuiDefinitions.Callback> GetAllResources()
        {
            var allResources = new Dictionary<ResourceType, GuiDefinitions.Callback>();

            allResources.Add(new AlcoholicDrink(), ButtonCallbacks.CBAlcoholicDrink);
            allResources.Add(new Bioplastic(), ButtonCallbacks.CBBioplastic);
            allResources.Add(new Gun(), ButtonCallbacks.CBGun);
            allResources.Add(new Meal(), ButtonCallbacks.CBMeal);
            allResources.Add(new MedicalSupplies(), ButtonCallbacks.CBMedicalSupplies);
            allResources.Add(new MedicinalPlants(), ButtonCallbacks.CBMedicinalPlants);
            allResources.Add(new Metal(), ButtonCallbacks.CBMetal);
            allResources.Add(new Ore(), ButtonCallbacks.CBOre);
            allResources.Add(new Semiconductors(), ButtonCallbacks.CBSemiconductors);
            allResources.Add(new Spares(), ButtonCallbacks.CBSpares);
            allResources.Add(new Starch(), ButtonCallbacks.CBStarch);
            allResources.Add(new Vegetables(), ButtonCallbacks.CBVegetables);
            allResources.Add(new Vitromeat(), ButtonCallbacks.CBVitromeat);

            return allResources;
        }
    }
    public static class ButtonCallbacks
    {
        public static void CBBioplastic(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Bioplastic().GetType());
        }
        public static void CBGun(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Gun().GetType());
        }
        public static void CBMeal(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Meal().GetType());
        }
        public static void CBMedicalSupplies(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new MedicalSupplies().GetType());
        }
        public static void CBMetal(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Metal().GetType());
        }
        public static void CBOre(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Ore().GetType());
        }
        public static void CBSpares(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Spares().GetType());
        }
        public static void CBStarch(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Starch().GetType());
        }
        public static void CBVegetables(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Vegetables().GetType());
        }
        public static void CBVitromeat(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Vitromeat().GetType());
        }
        public static void CBMedicinalPlants(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new MedicinalPlants().GetType());
        }
        public static void CBSemiconductors(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new Semiconductors().GetType());
        }
        public static void CBAlcoholicDrink(object parameter)
        {
            StorageGuru.GetInstance().StorageCallback(new AlcoholicDrink().GetType());
        }
    }
}
