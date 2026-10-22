using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FactorioModeler.Services
{
    public interface IWindowManager
    {
        void ShowSearchWindow(Window owner);
        void ShowSettingsWindow(Window owner);
        void CloseWindow(string windowName);
        bool IsWindowOpen(string windowName);
    }
}
