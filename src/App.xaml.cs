﻿using TimeManagementApp.Pages;

namespace TimeManagementApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
