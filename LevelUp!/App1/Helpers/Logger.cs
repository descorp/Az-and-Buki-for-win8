﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace LevelUP
{
    class Logger
    {
        public async static void ShowMessage(String MessageToShow)
        {
            var messageDialog = new MessageDialog(MessageToShow);

            messageDialog.Commands.Add(new UICommand("Дальше",
                new UICommandInvokedHandler(CommandInvokedHandler)));

            messageDialog.DefaultCommandIndex = 0;

            await messageDialog.ShowAsync();
        }
        
        private static void CommandInvokedHandler(IUICommand command)
        {
        }
    }
}