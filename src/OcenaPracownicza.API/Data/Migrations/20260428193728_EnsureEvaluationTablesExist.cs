using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OcenaPracownicza.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnsureEvaluationTablesExist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[EvaluationPeriods]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [EvaluationPeriods] (
                        [Id] uniqueidentifier NOT NULL,
                        [Name] nvarchar(max) NOT NULL,
                        [StartDate] datetime2 NOT NULL,
                        [EndDate] datetime2 NOT NULL,
                        [Regulation] nvarchar(max) NOT NULL,
                        [IsActive] bit NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_EvaluationPeriods] PRIMARY KEY ([Id])
                    );
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[EvaluationCriteria]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [EvaluationCriteria] (
                        [Id] uniqueidentifier NOT NULL,
                        [Name] nvarchar(max) NOT NULL,
                        [Description] nvarchar(max) NOT NULL,
                        [MinimumScore] int NOT NULL,
                        [EvaluationPeriodId] int NOT NULL,
                        [EvaluationPeriodId1] uniqueidentifier NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_EvaluationCriteria] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_EvaluationCriteria_EvaluationPeriods_EvaluationPeriodId1]
                            FOREIGN KEY ([EvaluationPeriodId1]) REFERENCES [EvaluationPeriods]([Id])
                    );
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[EvaluationCriteria]', N'U') IS NOT NULL
                    AND NOT EXISTS (
                        SELECT 1
                        FROM sys.indexes
                        WHERE [name] = N'IX_EvaluationCriteria_EvaluationPeriodId1'
                          AND [object_id] = OBJECT_ID(N'[EvaluationCriteria]')
                    )
                BEGIN
                    CREATE INDEX [IX_EvaluationCriteria_EvaluationPeriodId1]
                        ON [EvaluationCriteria]([EvaluationPeriodId1]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[EvaluationCriteria]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [EvaluationCriteria];
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[EvaluationPeriods]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [EvaluationPeriods];
                END
                """);
        }
    }
}
