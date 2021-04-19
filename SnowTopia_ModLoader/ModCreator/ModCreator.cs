using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnowTopia_ModLoader
{
    class ModCreator
    {
        string originalAssembly = @"C:\Users\luukw\Desktop\modding\mod_loader\Assembly-CSharp-original.dll";
        string moddedAssembly = "";
        string outFile = "";

        public ModCreator(string moddedAssembly, string outFile)
        {
            this.moddedAssembly = moddedAssembly;
            this.outFile = outFile;
        }

        public void CreateMod()
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDefMD moddedModule = ModuleDefMD.Load(moddedAssembly, modCtx);
            ModuleDefMD originalModule = ModuleDefMD.Load(originalAssembly, modCtx);

            #region QuickAndDirtyRemoveLater

            var changedMethods = GetChangedMethods(moddedModule, originalModule);

            var output = "";

            foreach (var method in changedMethods)
            {
                output += method.FullName + "\n";
            }

            var addedTypesAndMethods = GetAddedTypesAndMethods(moddedModule, originalModule);

            output += "\n\nADDED TYPES: \n\n";

            foreach (var addedType in addedTypesAndMethods.Item1)
            {
                output += addedType.FullName + "\n";
            }

            output += "\n\nADDED METHODS: \n\n";

            foreach (var addedMethod in addedTypesAndMethods.Item2)
            {
                output += addedMethod.FullName + "\n";
            }

            File.WriteAllText(outFile, output);

            #endregion

            var save = CreateSave(moddedModule, originalModule);
            var json = JsonConvert.SerializeObject(save, Formatting.Indented);
            File.WriteAllText(outFile, json);
        }

        private Save CreateSave(ModuleDefMD moddedModule, ModuleDefMD originalModule)
        {
            Save save = new Save();

            GetModdedMethods(moddedModule).ForEach(method => 
            {
                GetModMethodSave(method).ForEach(ModMethodSave =>
                {
                    save.ModdedMethods.Add(ModMethodSave);
                });
            });

            GetAddedTypes(moddedModule, originalModule).ForEach(type =>
            {
                var classSave = new NewClassSave
                {
                    FullName = type.FullName,
                    BaseType = type.BaseType.ToString(),
                    Attributes = type.Attributes.ToString()
                };

                foreach (var field in type.Fields)
                {
                    var newFieldSave = new NewFieldSave
                    {
                        FullName = field.FullName,
                        Attributes = field.Attributes.ToString()
                    };

                    classSave.Fields.Add(newFieldSave);
                }

                foreach (var method in type.Methods)
                {
                    var newFieldSave = new NewMethodSave
                    {
                        FullName = method.FullName,
                        Attributes = method.Attributes.ToString()
                    };

                    foreach (var instruction in method.Body.Instructions)
                    {
                        newFieldSave.Instructions.Add(instruction.ToString());
                    }

                    classSave.Methods.Add(newFieldSave);
                }

                save.NewClasses.Add(classSave);
            });

            return save;
        }

        private List<TypeDef> GetAddedTypes(ModuleDefMD moddedModule, ModuleDefMD originalModule)
        {
            var addedTypes = new List<TypeDef>();

            var typeFound = false;

            foreach (var typeDef in moddedModule.GetTypes())
            {
                foreach (var orgTypeDef in originalModule.GetTypes())
                {
                    if(typeDef.FullName == orgTypeDef.FullName)
                    {
                        typeFound = true;
                        break;
                    }
                }

                if (typeFound == false)
                    addedTypes.Add(typeDef);
                else
                    typeFound = false;
            }

            return addedTypes;
        }

        private List<MethodDef> GetModdedMethods(ModuleDefMD module)
        {
            var moddedMethods = new List<MethodDef>();
            var types = module.GetTypes();

            foreach (var type in types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body == null)
                        continue;

                    foreach (var instruction in method.Body.Instructions)
                    {                        
                        if (OpIsModMarker(instruction))
                        {
                            moddedMethods.Add(method);
                            break;
                        }
                    }
                }
            }

            return moddedMethods;
        }

        #region QuickAndDirtyRemoveLater
        private List<MethodDef> GetChangedMethods(ModuleDefMD module, ModuleDefMD originalModule)
        {
            var moddedMethods = new List<MethodDef>();
            var originalTypes = originalModule.GetTypes().ToList();
            var moddedTypes = module.GetTypes().ToList();

            foreach (var originalTypeDef in originalTypes)
            {
                var moddedTypeDef = moddedTypes.Where(x => x.FullName == originalTypeDef.FullName).FirstOrDefault();

                if (moddedTypeDef == null)
                    continue;

                foreach (var originalMethod in originalTypeDef.Methods)
                {
                    var moddedMethod = moddedTypeDef.Methods.Where(x => x.FullName == originalMethod.FullName).FirstOrDefault();

                    if (originalMethod.Body == null || moddedMethod == null)
                        continue;

                    if(originalMethod.Body.Instructions.Count != moddedMethod.Body.Instructions.Count)
                    {
                        moddedMethods.Add(moddedMethod);
                        continue;
                    }

                    for (int i = 0; i < originalMethod.Body.Instructions.Count; i++)
                    {
                        if(originalMethod.Body.Instructions[i].ToString() != moddedMethod.Body.Instructions[i].ToString())
                        {
                            moddedMethods.Add(moddedMethod);
                            break;
                        }
                    }
                }

            }

            return moddedMethods;
        }

        private (List<TypeDef>, List<MethodDef>) GetAddedTypesAndMethods(ModuleDefMD module, ModuleDefMD originalModule)
        {
            var addedMethods = new List<MethodDef>();
            var addedTypes = new List<TypeDef>();

            var originalTypes = originalModule.GetTypes().ToList();
            var moddedTypes = module.GetTypes().ToList();

            foreach (var moddedTypeDef in moddedTypes)
            {
                var originalTypeDef = originalTypes.Where(x => x.FullName == moddedTypeDef.FullName).FirstOrDefault();

                if(originalTypeDef == null)
                {
                    addedTypes.Add(moddedTypeDef);
                    continue;
                }

                foreach (var moddedMethod in moddedTypeDef.Methods)
                {
                    var originalMethod = originalTypeDef.Methods.Where(x => x.FullName == moddedMethod.FullName).FirstOrDefault();

                    if(originalMethod == null)
                    {
                        addedMethods.Add(moddedMethod);
                    }

                }
            }


            return (addedTypes, addedMethods);
        }
        #endregion

        private List<ModdedMethodSave> GetModMethodSave(MethodDef method)
        {
            var modMethodInstructions = new List<ModdedMethodSave>();
            ModdedMethodSave modMethodInstruction = new ModdedMethodSave();
            var instructions = method.Body.Instructions;
            var recording = false;

            for (int i = 0; i < instructions.Count; i++)
            {
                // Start
                if (OpIsModMarker(instructions[i]) && !recording)
                {
                    modMethodInstruction = new ModdedMethodSave
                    {
                        FullName = method.FullName
                    };

                    if(!OpIsEmptyModMarker(instructions[i]))
                    {
                        modMethodInstruction.Start = instructions[i - 1].Operand.ToString();
                    }
                    else
                    {
                        modMethodInstruction.Start = "";
                    }

                    recording = true;
                    continue;
                }

                // Recording
                if (recording && !OpIsModMarker(instructions[i]))
                {
                    modMethodInstruction.Instructions.Add(instructions[i].ToString());
                }

                // End
                if (OpIsModMarker(instructions[i]) && recording)
                {
                    if (!OpIsEmptyModMarker(instructions[i]))
                    {
                        modMethodInstruction.End = instructions[i - 1].Operand.ToString();
                        
                        if(modMethodInstruction.Instructions.Last() == instructions[i - 1].ToString())
                            modMethodInstruction.Instructions.RemoveAt(modMethodInstruction.Instructions.Count - 1);
                    }
                    else
                    {
                        modMethodInstruction.End = "";
                    }

                    modMethodInstructions.Add(modMethodInstruction);

                    recording = false;
                    continue;
                }
            }

            return modMethodInstructions;
        }

         

        private string GetOperandString(Instruction i) => i.Operand != null ? i.Operand.ToString() : "";

        private bool OpIsModMarker(Instruction i) => GetOperandString(i) == "System.Void Core::mod_marker(System.String)" || GetOperandString(i) == "System.Void Core::mod_marker()";

        private bool OpIsEmptyModMarker(Instruction i) => GetOperandString(i) == "System.Void Core::mod_marker()";

    }
}
