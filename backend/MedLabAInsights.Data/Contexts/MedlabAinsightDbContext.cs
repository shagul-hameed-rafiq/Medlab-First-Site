using Microsoft.EntityFrameworkCore;
using MedLabAInsights.Models;

namespace MedLabAInsights.Data.Contexts
{
    public class MedlabAinsightDbContext : DbContext
    {
        public MedlabAinsightDbContext(DbContextOptions<MedlabAinsightDbContext> options)
            : base(options)
        {
        }

        public DbSet<Panel> Panels => Set<Panel>();
        public DbSet<Test> Tests => Set<Test>();
        public DbSet<PanelTestMapping> PanelTestMappings => Set<PanelTestMapping>();
        public DbSet<BandRuleReport> BandRuleReports => Set<BandRuleReport>();
        public DbSet<PanelRuleSummary> PanelRuleSummaries => Set<PanelRuleSummary>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Visit> Visits => Set<Visit>();
        public DbSet<VisitTestResult> VisitTestResults => Set<VisitTestResult>();
        public DbSet<VisitTestInterpretation> VisitTestInterpretations => Set<VisitTestInterpretation>();
        public DbSet<VisitPanelSummary> VisitPanelSummaries => Set<VisitPanelSummary>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically picks up all IEntityTypeConfiguration<T> in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedlabAinsightDbContext).Assembly);
        }
    }
}
