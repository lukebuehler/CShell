using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using CShell.Util;

namespace CShell.Modules.Workspace.ViewModels
{
    public class AssemblyGacItemViewModel : PropertyChangedBase
    {
        private readonly AssemblyName assemblyName;
        public AssemblyGacItemViewModel(AssemblyName assemblyName)
        {
            this.assemblyName = assemblyName;
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; NotifyOfPropertyChange(()=>IsSelected);}
        }

        public string Name
        {
            get { return AssemblyName.Name; }
        }

        public string Version
        {
            get { return AssemblyName.Version.ToString(); }
        }

        private string filePath = null;
        public string FilePath
        {
            get
            {
                return filePath ?? (filePath = GlobalAssemblyCache.FindAssemblyInNetGac(AssemblyName));
            }
        }

        public AssemblyName AssemblyName
        {
            get { return assemblyName; }
        }
    }

    public class AssemblyGacViewModel : Screen
    {
        public AssemblyGacViewModel()
        {
            DisplayName = "Add Reference from GAC";
            MaxSelectedAssemblyCount = 20;
            LatestVersionsOnly = true;
        }

        private List<AssemblyGacItemViewModel> gacItems;
        private List<AssemblyGacItemViewModel> GacItems
        {
            get
            {
                if(gacItems == null)
                {
                    gacItems = new List<AssemblyGacItemViewModel>();
                    foreach (var assemblyName in GlobalAssemblyCache.GetAssemblyList())
                    {
                        var vm = new AssemblyGacItemViewModel(assemblyName);
                        vm.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == "IsSelected")
                            {
                                NotifyOfPropertyChange(() => SelectedAssemblies);
                                NotifyOfPropertyChange(() => SelectedAssemblyCount);
                                NotifyOfPropertyChange(() => CanOk);
                            }
                        };
                        gacItems.Add(vm);
                    }
                }
                return gacItems;
            }
        }

        public IEnumerable<AssemblyGacItemViewModel> Assemblies
        {
            get
            {
                IEnumerable<AssemblyGacItemViewModel> items = GacItems;
                if (LatestVersionsOnly)
                    items = items.GroupBy(item => item.Name).Select(group => group.OrderByDescending(item => item.AssemblyName.Version).First());

                if (String.IsNullOrEmpty(searchText))
                    return items;
                else
                    return items.Where(item => item.Name.ToLower().Contains(searchText.ToLower()));
            }
        }

        public IEnumerable<AssemblyGacItemViewModel> SelectedAssemblies
        {
            get { return GacItems.Where(item => item.IsSelected); }
        }

        public int SelectedAssemblyCount
        {
            get { return SelectedAssemblies.Count(); }
        }

        /// <summary>
        /// Gets or sets the max count of allowed assemblies to be selected at once.
        /// </summary>
        public int MaxSelectedAssemblyCount { get; set; }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                NotifyOfPropertyChange(() => Assemblies);
            }
        }

        private bool latestVersionsOnly;
        public bool LatestVersionsOnly
        {
            get { return latestVersionsOnly; }
            set
            {
                latestVersionsOnly = value;
                NotifyOfPropertyChange(() => LatestVersionsOnly);
                NotifyOfPropertyChange(() => Assemblies);
            }
        }

        public bool CanOk
        {
            get { return SelectedAssemblies.Any() && SelectedAssemblyCount <= MaxSelectedAssemblyCount; }
        }

        public void Ok()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}
