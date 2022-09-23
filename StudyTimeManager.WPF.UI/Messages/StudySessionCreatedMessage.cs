﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace StudyTimeManager.WPF.UI.Messages
{
    public class StudySessionCreatedMessage : ValueChangedMessage<Guid>
    {
        public StudySessionCreatedMessage(Guid value) : base(value)
        {
        }
    }
}
