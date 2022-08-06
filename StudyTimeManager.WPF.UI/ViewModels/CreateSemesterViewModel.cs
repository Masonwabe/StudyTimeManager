﻿using CommunityToolkit.Mvvm.ComponentModel;
using StudyTimeManager.Domain.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using StudyTimeManager.WPF.UI.Stores;
using StudyTimeManager.Domain.Services.Contracts;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StudyTimeManager.WPF.UI.Messages;

namespace StudyTimeManager.WPF.UI.ViewModels;
public partial class CreateSemesterViewModel : ObservableRecipient
{
    public MessageHandler<CreateModuleViewModel, SemesterCreatedMessage> SemesterCreated;

    //[Required(ErrorMessage ="Semester start date is required")]
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    private DateTime _startDate;

    //[Required]
    //[Range(1, int.MaxValue,ErrorMessage ="Number of weeks must be min. 1")]
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    private int _numberOfWeeks;

    public bool CanCreate => _numberOfWeeks>0 && !string.IsNullOrWhiteSpace(_startDate.ToString());

    private readonly IServiceManager _service;
    private readonly CreateModuleViewModel _createModuleViewModel;

    public ICommand CreateSemesterCommand { get; }

    public CreateSemesterViewModel(IServiceManager service, CreateModuleViewModel createModuleViewModel)
    {
        Semester semester = new Semester();
        //startDate = DateOnly.FromDateTime(DateTime.NowDateTime.Now);
        _startDate = DateTime.Now;
        _service = service;
        _createModuleViewModel = createModuleViewModel;

        //CreateSemesterCommand(CreateSemesterr);
        CreateSemesterCommand = new RelayCommand(CreateSemesterr);

        
    }

    [RelayCommand]
    public void CreateSemesterr()
    {
        Semester semester = new Semester()
        {
            NumberOfWeeks = _numberOfWeeks,
            StartDate = DateOnly.FromDateTime(_startDate)
        };

        bool successful = _service.SemesterService.CreateSemester(semester);

        if (successful)
        {
            SemesterCreatedMessage message = new SemesterCreatedMessage(successful);
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}