using _2048.Stores;
using _2048.Models;
using _2048.ViewModels;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using _2048.Services;
using _2048.Models.Database;
using _2048.Models.GameLogic.Achievements;
using _2048.Models.GameLogic.Achievements.Checkers;

namespace _2048
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App()
        {
            IServiceCollection services = new ServiceCollection();

            AddDatabaseServices(ref services);
            AddAchievementServices(ref services);
            AddViewModels(ref services);
            AddNavigationServices(ref services);

            _serviceProvider = services.BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            db.Database.EnsureCreated();

            DatabaseSeeder.Seed(db);
        }

        private static void AddAchievementServices(ref IServiceCollection services)
        {
            services.AddSingleton<GameStateStore>();

            services.AddTransient<ReachTileAchievementChecker>();
            services.AddTransient<ReachTileWithinMinuteAchievementChecker>();
            services.AddTransient<MovesCountAchievementChecker>();
            services.AddTransient<TimePlayedAchievementChecker>();

            services.AddSingleton<AchievementCheckerFactory>();
            services.AddSingleton<AchievementManager>();
        }

        private static void AddDatabaseServices(ref IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlite("Data Source=game2048.db"));

            services.AddScoped<GameService>();
            services.AddScoped<PlayerService>();
            services.AddScoped<ScoreboardService>();
            services.AddScoped<AchievementsService>();
        }

        private static void AddViewModels(ref IServiceCollection services)
        {
            services.AddSingleton<MainViewModel>();
            services.AddSingleton(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });
            services.AddTransient<TileViewModel>();
            services.AddTransient<GameViewModel>();
            services.AddTransient<AboutViewModel>();
            services.AddTransient<AchievementsViewModel>();
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<ScoreboardViewModel>();
        }

        private static void AddNavigationServices(ref IServiceCollection services)
        {
            services.AddSingleton<NavigationStore>();

            services.AddTransient(s =>
                new NavigationService<GameViewModel>(
                    s.GetRequiredService<NavigationStore>(),
                    () => s.GetRequiredService<GameViewModel>()
            ));

            services.AddTransient(s =>
                new NavigationService<ScoreboardViewModel>(
                    s.GetRequiredService<NavigationStore>(),
                    () => s.GetRequiredService<ScoreboardViewModel>()
                ));

            services.AddTransient(s =>
                new NavigationService<AchievementsViewModel>(
                    s.GetRequiredService<NavigationStore>(),
                    () => s.GetRequiredService<AchievementsViewModel>()
                ));

            services.AddTransient(s =>
                new NavigationService<AboutViewModel>(
                    s.GetRequiredService<NavigationStore>(),
                    () => s.GetRequiredService<AboutViewModel>()
                ));

            services.AddTransient(s =>
                new NavigationService<MainMenuViewModel>(
                    s.GetRequiredService<NavigationStore>(),
                    () => s.GetRequiredService<MainMenuViewModel>()
                ));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            NavigationStore navigationStore = _serviceProvider.GetRequiredService<NavigationStore>();
            navigationStore.CurrentViewModel = _serviceProvider.GetRequiredService<MainMenuViewModel>();

            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
