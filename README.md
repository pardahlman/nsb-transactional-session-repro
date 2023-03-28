# NServiceBus Transactional Session repro app

When using [Transactional session](https://docs.particular.net/nservicebus/transactional-session/) with the
outbox [configured to use `TransactionScope`](https://docs.particular.net/persistence/sql/outbox#transaction-type-transaction-scope)
the ambient transaction `System.Transactions.Transaction.Current` is `null` after opening a transactional session.

Start PostgreSQL in Docker:

```
docker run --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres
```

Run the application

```
dotnet run --project .\NsbRepro\
```

Output:

```
Transaction.Current: null
```