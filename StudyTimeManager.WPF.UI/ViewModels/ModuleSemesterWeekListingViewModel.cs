﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StudyTimeManager.Domain.Models;
using StudyTimeManager.Domain.Services.Contracts;
using StudyTimeManager.WPF.UI.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StudyTimeManager.WPF.UI.ViewModels;
public partial class ModuleSemesterWeekListingViewModel : ObservableObject, 
    IRecipient<StudySessionCreatedMessage>, 
    IRecipient<SelectedModuleListingItemViewModelChangedMessage>
{
    private readonly IServiceManager _service;
    private readonly ObservableCollection<ModuleSemesterWeekListingItemViewModel> _moduleSemesterListingItems;

    public IEnumerable<ModuleSemesterWeekListingItemViewModel>
        ModuleSemesterWeekListingItemViewModel => _moduleSemesterListingItems;

    [ObservableProperty]
    private ModuleListingItemViewModel? _module;

    [ObservableProperty]
    private bool _canDelete;

    public ModuleSemesterWeekListingViewModel(IServiceManager service)
    {
        _service = service;
        _moduleSemesterListingItems = new ObservableCollection<ModuleSemesterWeekListingItemViewModel>();
        CanDelete = false;
        RegisterToMessages();
    }

    [RelayCommand]
    private void DeleteModule()
    {
        bool isDeleted = _service.ModuleService.DeleteModule(Module.ModuleCode);
        if (isDeleted)
        {
            ModuleDeletedMessage message = new ModuleDeletedMessage(Module);
            WeakReferenceMessenger.Default.Send(message);

            _moduleSemesterListingItems.Clear();
            Module = null;
            CanDelete = false;
        }
    }

    private void UpdateListing()
    {
        _moduleSemesterListingItems.Clear();
        if (Module != null)
        {
            CanDelete = true;
            ICollection<ModuleSemesterWeek> semesterWeeks = _service.ModuleSemesterWeekService
                .GetModuleSemesterWeeksForAModule(Module.ModuleCode);

            foreach (var semesterWeek in semesterWeeks)
            {
                _moduleSemesterListingItems
                    .Add(new ModuleSemesterWeekListingItemViewModel(semesterWeek));
            }
        }
    }

    public void RegisterToMessages()
    {
        WeakReferenceMessenger.Default
            .Register<SelectedModuleListingItemViewModelChangedMessage>(this);
        WeakReferenceMessenger.Default.Register<StudySessionCreatedMessage>(this);
    }

    public void Receive(SelectedModuleListingItemViewModelChangedMessage message)
    {
        Module = message.Value;
        UpdateListing();
    }

    public void Receive(StudySessionCreatedMessage message)
    {
        if (Module.ModuleCode.Equals(message.Value))
        {
            UpdateListing();
        }
    }
}
