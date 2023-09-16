using ToDoList.Controls;
using ToDoList.Interfaces;

namespace ToDoList.Services
{
    public class DialogService : IDialogService
    {
        public bool? Show(string itemName)
        {
            ConfirmationDialog confirmationDialog = new ConfirmationDialog(itemName);
            return confirmationDialog.ShowDialog();
        }
    }
}
