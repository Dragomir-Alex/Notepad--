using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.ObjectModel;
using static Notepad__.Model.DataProvider;
using Notepad__.Model;
using Notepad__.View;
using System.Windows;
using System.Text.RegularExpressions;

namespace Notepad__.Commands
{
    class FileCommands : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<TabModel> Tabs { get; set; }

        private int currentTab;
        public int CurrentTab { get => currentTab; set => SetProperty(ref currentTab, value); }


        private ICommand m_new;
        private ICommand m_open;
        private ICommand m_save;
        private ICommand m_saveAs;
        private ICommand m_exit;
        private ICommand m_closeTab;
        private ICommand m_find;
        private ICommand m_replace;
        private ICommand m_replaceAll;
        private ICommand m_about;

        public FileCommands()
        {
            Tabs = new ObservableCollection<TabModel>();
            Tabs.Add(new TabModel { FileName = "File 1", Content = "", FilePath = "", Saved = true });
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void New(object parameter)
        {
            Tabs.Add(new TabModel { FileName = "File " + (Tabs.Count + 1), Content = "", FilePath = "", Saved = true });
            currentTab = Tabs.Count() - 1;
            OnPropertyChanged("CurrentTab");
        }

        public void Open(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a text file...",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Stream fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    Tabs.Add(new TabModel { FileName = openFileDialog.SafeFileName, Content = reader.ReadToEnd() });
                    OnPropertyChanged("Tabs");
                }
                currentTab = Tabs.Count() - 1;
                Tabs[currentTab].FilePath = openFileDialog.FileName;
                Tabs[currentTab].Saved = true;
                OnPropertyChanged("CurrentTab");
            }
        }

        public void Save(object parameter)
        {
            if (Tabs[currentTab].FilePath == "")
            {
                SaveAs(parameter);
                return;
            }
            StreamWriter writer = new StreamWriter(Tabs[currentTab].FilePath);
            writer.Write(Tabs[currentTab].Content);
            writer.Close();
            Tabs[currentTab].Saved = true;
        }

        public void SaveAs(object parameter)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save...",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, Tabs[currentTab].Content);
                Tabs[currentTab].FileName = saveFileDialog.SafeFileName;
                Tabs[currentTab].FilePath = saveFileDialog.FileName;
                Tabs[currentTab].Saved = true;
            }
            OnPropertyChanged("Tabs");
        }

        private void Find(object parameter)
        {
            var searchBox = new SearchBox();
            var parameters = new List<string>() { "Find..." };

            searchBox.CreateSearchBox(parameters, true);
            searchBox.ShowDialog();

            string find = searchBox.GetInputList()[0];
            string searchAll = searchBox.GetInputList()[1];

            if (find == "")
                return;

            if (searchAll == "True")
            {
                bool foundInOne = false;

                foreach (var Tab in Tabs)
                {
                    int startIndex = Tab.Content.IndexOf(find);
                    if (startIndex == -1)
                        continue;
                    else
                    {
                        foundInOne = true;
                        Tab.SelectionStart = startIndex;
                        Tab.SelectionLength = find.Length;
                    }
                }
                if (foundInOne)
                    MessageBox.Show("Found!");
                else MessageBox.Show("Not found!");
            }

            else
            {
                int startIndex = Tabs[currentTab].Content.IndexOf(find);
                if (startIndex == -1)
                {
                    MessageBox.Show("Not found!");
                    return;
                }
                else
                {
                    MessageBox.Show("Found!");
                    Tabs[currentTab].SelectionStart = startIndex;
                    Tabs[currentTab].SelectionLength = find.Length;
                }
            }

            OnPropertyChanged("Tabs");
        }

        private void Replace(object parameter)
        {
            var searchBox = new SearchBox();
            var parameters = new List<string>() { "Find...", "Replace with..." };

            searchBox.CreateSearchBox(parameters, false);
            searchBox.ShowDialog();

            string find = searchBox.GetInputList()[0];
            string replace = searchBox.GetInputList()[1];

            if (find == "" || replace == "")
                return;

            string changedText;
            var regex = new Regex(Regex.Escape(find));
            changedText = regex.Replace(Tabs[currentTab].Content, replace, 1);
            if (changedText != Tabs[currentTab].Content)
                Tabs[currentTab].Content = changedText;
            OnPropertyChanged("Tabs");
        }

        private void ReplaceAll(object parameter)
        {
            var searchBox = new SearchBox();
            var parameters = new List<string>() { "Find...", "Replace with..." };

            searchBox.CreateSearchBox(parameters, true);
            searchBox.ShowDialog();

            string find = searchBox.GetInputList()[0];
            string replace = searchBox.GetInputList()[1];
            string searchAll = searchBox.GetInputList()[2];

            if (find == "" || replace == "")
                return;

            string changedText;
            if (searchAll == "True")
                foreach (var Tab in Tabs)
                {
                    changedText = Tab.Content.Replace(find, replace);
                    if (changedText != Tab.Content)
                        Tab.Content = changedText;
                }            
            else
            {
                changedText = Tabs[currentTab].Content = Tabs[currentTab].Content.Replace(find, replace);
                if (changedText != Tabs[currentTab].Content)
                    Tabs[currentTab].Content = changedText;
            }                
            OnPropertyChanged("Tabs");
        }

        private void About(object parameter)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void CloseTab(object parameter)
        {
            var selectedTab = parameter as TabModel;
            if (selectedTab.Saved == false)
            {
                var result = MessageBox.Show("You have unsaved changes. Close the tab anyway?", "Warning", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    Tabs.Remove(parameter as TabModel);
                else return;
            }
            else Tabs.Remove(parameter as TabModel);
        }

        private void Exit(object parameter)
        {
            bool foundUnsaved = false;
            foreach (var Tab in Tabs)
                if (Tab.Saved == false)
                {
                    foundUnsaved = true;
                    break;
                }

            if (foundUnsaved)
            {
                var result = MessageBox.Show("You have unsaved changes. Close the program anyway?", "Warning", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    Environment.Exit(0);
                else return;
            }
            else Environment.Exit(0);
        }

        public ICommand CommandNew
        {
            get
            {
                if (m_new == null)
                    m_new = new RelayCommand(New);
                return m_new;
            }
        }

        public ICommand CommandOpen
        {
            get
            {
                if (m_open == null)
                    m_open = new RelayCommand(Open);
                return m_open;
            }
        }

        public ICommand CommandSave
        {
            get
            {
                if (m_save == null)
                    m_save = new RelayCommand(Save);
                return m_save;
            }
        }

        public ICommand CommandSaveAs
        {
            get
            {
                if (m_saveAs == null)
                    m_saveAs = new RelayCommand(SaveAs);
                return m_saveAs;
            }
        }

        public ICommand CommandExit
        {
            get
            {
                if (m_exit == null)
                    m_exit = new RelayCommand(Exit);
                return m_exit;
            }
        }

        public ICommand CommandFind
        {
            get
            {
                if (m_find == null)
                    m_find = new RelayCommand(Find);
                return m_find;
            }
        }

        public ICommand CommandReplace
        {
            get
            {
                if (m_replace == null)
                    m_replace = new RelayCommand(Replace);
                return m_replace;
            }
        }
        public ICommand CommandReplaceAll
        {
            get
            {
                if (m_replaceAll == null)
                    m_replaceAll = new RelayCommand(ReplaceAll);
                return m_replaceAll;
            }
        }

        public ICommand CommandAbout
        {
            get
            {
                if (m_about == null)
                    m_about = new RelayCommand(About);
                return m_about;
            }
        }

        public ICommand CommandCloseTab
        {
            get
            {
                if (m_closeTab == null)
                    m_closeTab = new RelayCommand(parameter => this.CloseTab(parameter));
                return m_closeTab;
            }
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }
    }
}
