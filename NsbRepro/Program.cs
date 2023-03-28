using System;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NServiceBus;
using NServiceBus.TransactionalSession;

var endpointConfig = new EndpointConfiguration("TestEndpoint");
endpointConfig.UseTransport<LearningTransport>().Transactions(TransportTransactionMode.ReceiveOnly);
// Configure persistence
var persistence = endpointConfig.UsePersistence<SqlPersistence>();
persistence.SqlDialect<SqlDialect.PostgreSql>();
persistence.EnableTransactionalSession();
persistence.ConnectionBuilder(() => new NpgsqlConnection("User ID=postgres;Password=postgres;Host=localhost;Port=5432"));
// Configure outbox with TransactionScope
var outbox = endpointConfig.EnableOutbox();
outbox.UseTransactionScope();
// Configure and start endpoint
var services = new ServiceCollection();
var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfig, services);
var serviceProvider = services.BuildServiceProvider();
await startableEndpoint.Start(serviceProvider);

// Start a transactional session. When debugging, it is clear that an TransactionScopeSqlOutboxTransaction is used and
// that it creates a new TransactionScope. However, the ambient transaction is null here.
var transactionalSession = serviceProvider.GetRequiredService<ITransactionalSession>();
await transactionalSession.Open(new SqlPersistenceOpenSessionOptions());
Console.Out.WriteLine("Transaction.Current: {0}", Transaction.Current != null ? Transaction.Current : "null");
