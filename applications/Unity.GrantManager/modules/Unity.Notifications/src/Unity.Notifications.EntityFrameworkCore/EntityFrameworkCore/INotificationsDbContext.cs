﻿using Microsoft.EntityFrameworkCore;
using Unity.Notifications.Emails;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Unity.Notifications.EntityFrameworkCore;

[ConnectionStringName(NotificationsDbProperties.ConnectionStringName)]
public interface INotificationsDbContext : IEfCoreDbContext
{
    // Add DbSet for each Aggregate Root here.
    public DbSet<EmailLog> EmailLogs { get; set; }
}
