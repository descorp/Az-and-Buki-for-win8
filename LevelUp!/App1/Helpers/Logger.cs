using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

namespace levelupspace
{
    class Logger
    {
        public async static void ShowMessage(String MessageToShow)
        {
            var messageDialog = new MessageDialog(MessageToShow);
            var res = new ResourceLoader();

            messageDialog.Commands.Add(new UICommand(res.GetString("btnNextContent"),
                CommandInvokedHandler));

            messageDialog.DefaultCommandIndex = 0;

            await messageDialog.ShowAsync();
        }
        
        private static void CommandInvokedHandler(IUICommand command)
        {
        }
    }
}
