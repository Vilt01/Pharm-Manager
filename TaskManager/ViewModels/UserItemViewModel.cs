using TaskManager.Models;

namespace TaskManager.ViewModels;

public class UserItemViewModel : ItemViewModel<User>
{
    public UserItemViewModel(User user) : base(user) { }

    // Удобные свойства для View (чтобы не писать сложные binding'и)
    public string FullName => $"{Model.Surname} {Model.Name} {Model.Lastname}".Trim();
    public string RoleName => Model.FkRoleNavigation?.Name ?? "—";
    public bool IsActive => Model.IsDeleted != true;
}