using HomeNet.Web.Cqrs;

namespace HomeNet.Web.Extensions;

public static class FinancesModuleExtensions
{
    public static IServiceCollection AddFinancesModule(
        this IServiceCollection services)
    {
        // services
        //     .AddScoped<ICategoryRepository, CategoryRepository>()
        //     .AddScoped<ITransactionRepository, TransactionRepository>();
        // services.AddSingleton<IMonthlyTimelineRepository, MonthlyTimelineCache>();

        // services.AddScoped<ITimelineBuilder, TimelineBuilder>();

        // services
        //     .AddTransient<AddCategoryCommandHandler>()
        //     .AddTransient<AddExpenseCommandHandler>()
        //     .AddTransient<AddIncomeCommandHandler>();
        
        // services
        //     .AddTransient<CategoriesQueryHandler>()
        //     .AddTransient<ExpensesQueryHandler>()
        //     .AddTransient<IncomesQueryHandler>()
        //     .AddTransient<MonthlyTimelineQueryHandler>();

        return services;
    }

    public static ICqrsBuilder AddFinancesModule(this ICqrsBuilder builder)
    {
        // builder.AddCommand<AddCategoryCommand, AddCategoryCommandHandler>();
        // builder.AddCommand<AddExpenseCommand, AddExpenseCommandHandler>();
        // builder.AddCommand<AddIncomeCommand, AddIncomeCommandHandler>();

        // builder.AddQuery<CategoriesQuery, CategoriesQueryHandler, IReadOnlyList<Category>>();
        // builder.AddQuery<ExpensesQuery, ExpensesQueryHandler, IReadOnlyList<Expense>>();
        // builder.AddQuery<IncomesQuery, IncomesQueryHandler, IReadOnlyList<Income>>();
        // builder.AddQuery<MonthlyTimelineQuery, MonthlyTimelineQueryHandler, MonthlyTimeline>();

        return builder;
    }
}
