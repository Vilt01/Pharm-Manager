using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.ViewModels;

public abstract class ItemViewModel<T> : INotifyPropertyChanged where T : class
{
    public T Model { get; }

    protected ItemViewModel(T model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}