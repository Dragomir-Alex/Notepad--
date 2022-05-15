using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Notepad__.Model
{
    internal class TabModel : INotifyPropertyChanged
    {
        private string filename, content;
        private bool saved;
        private int selectionStart, selectionLength;
        public string FilePath { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public string FileName { get => filename; set => SetProperty(ref filename, value); }
        public string Content 
        { 
            get => content;
            set
            {
                SetProperty(ref content, value);
                Saved = false;
            }
        }
        public int SelectionStart { get => selectionStart; set => SetProperty(ref selectionStart, value); }
        public int SelectionLength { get => selectionLength; set => SetProperty(ref selectionLength, value); }
        public bool Saved { get => saved; set => SetProperty(ref saved, value); }

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
