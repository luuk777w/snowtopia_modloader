using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace SnowTopia_ModLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Mod> mods = new List<Mod>();

        public MainWindow()
        {
            InitializeComponent();

            string moddedAssemblyx = @"D:\SteamLibrary\steamapps\common\Snowtopia\Snowtopia_Data\Managed\Assembly-CSharp.dll";
            string outFilex = @"D:\SteamLibrary\steamapps\common\Snowtopia\Mods\test.stmod";

            //var modCreator = new ModCreator(moddedAssemblyx, outFilex);
            //modCreator.CreateMod();
            //this.Close();

            

            if(!SettingsOk)
            {
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.ShowDialog();

                if(!SettingsOk)
                    Application.Current.Shutdown();
            }


            var modFiles = Directory.GetFiles(Settings.Default.ModFolderLocation);

            foreach (var mod in modFiles)
            {
                mods.Add(new Mod() { 
                    Path = mod,
                    Name = System.IO.Path.GetFileNameWithoutExtension(mod), 
                    Active = false });
            }

            dataGrid.ItemsSource = mods;

            //var modLoader = new ModLoader();
            //modLoader.LoadMods(mods);
            //this.Close();

            

            #region testing

            /*


            var text = "";

            var originalAssembly = @"C:\Users\luukw\Desktop\modding\mod_loader\Assembly-CSharp-original.dll";
            var moddedAssembly = @"D:\SteamLibrary\steamapps\common\Snowtopia\Snowtopia_Data\Managed\Assembly-CSharp.dll";
            var outFile = @"C:\Users\luukw\Desktop\modding\mod_loader\Assembly-CSharp.dll";
            
            ModuleContext modCtx = ModuleDef.CreateModuleContext();

            ModuleDefMD originalModule = ModuleDefMD.Load(originalAssembly, modCtx);
            MethodDef originalMethod = getMethod(originalModule, "CoreModule", "System.Void CoreModule::Start()");
            var originalInstructions = originalMethod.Body.Instructions;

            ModuleDefMD moddedModule = ModuleDefMD.Load(moddedAssembly, modCtx);
            MethodDef moddedMethod = getMethod(moddedModule, "CoreModule", "System.Void CoreModule::Start()");
            var moddedInstructions = moddedMethod.Body.Instructions;

            var recording_mod = false;
            var modInstructions = new List<Instruction>();

            foreach (var instruction in moddedInstructions)
            {
                if(modInstructions.Count == 3)
                {
                   
                }

                if (recording_mod && !OpIsModAdd(instruction))
                {
                    instruction.Offset -= 5;

                    if(instruction.Operand != null)
                    {
                        var x = instruction.Operand.GetType();

                        if (instruction.Operand.GetType() == typeof(SByte))
                        {
                            var newInstruction = instruction.OpCode.ToInstruction((SByte)instruction.Operand);
                            modInstructions.Add(newInstruction);
                        }
                        else
                        {
                            foreach (var type in originalModule.GetTypes())
                            {
                                if (type.Fields.Any(x => x.FullName == instruction.Operand.ToString()))
                                {
                                    var operand = type.Fields.Where(x => x.FullName == instruction.Operand.ToString()).FirstOrDefault();
                                    var newInstruction = instruction.OpCode.ToInstruction(operand);
                                    newInstruction.Offset = instruction.Offset;
                                    modInstructions.Add(newInstruction);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var newInstruction = instruction.OpCode.ToInstruction();
                        modInstructions.Add(newInstruction);
                    }

                }

                if (OpIsModAdd(instruction))
                    recording_mod = !recording_mod;
            }

            var newMethod = new List<Instruction>();
            var modAdded = false;

            foreach (var instruction in originalInstructions)
            {
                if(instruction.Offset >= modInstructions.First().Offset && !modAdded)
                {
                    newMethod.AddRange(modInstructions);
                    modAdded = true;
                }

                instruction.Offset += modInstructions.Last().Offset + 5;
                newMethod.Add(instruction);
            }

            originalInstructions.Clear();
            //originalMethod.Body.KeepOldMaxStack = true;

            foreach (var instruction in newMethod)
            {
                originalInstructions.Add(instruction);
            }

            foreach (var instruction in originalInstructions)
            {
                text += instruction + "\n";
            }

            //originalModule.Write(outFile);

            */

            #endregion
        }

        private void ButtonGoToModCreationOnClick(object sender, RoutedEventArgs e)
        {
            var ModCreation = new ModCreation();
            ModCreation.Show();
        }

        private void OpenSettingsOnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private bool SettingsOk => File.Exists(Settings.Default.AssemblyLocation) && Directory.Exists(Settings.Default.ModFolderLocation);

        private void LoadMods(object sender, RoutedEventArgs e)
        {
            ModLoader modLoader = new ModLoader(Settings.Default.AssemblyLocation);

            modLoader.LoadMods(mods);

            MessageBox.Show("Mods Loaded Succesfully!", "Snowtopia Mod Loader", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
