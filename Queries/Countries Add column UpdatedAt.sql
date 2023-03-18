alter table dbo.countries
add [UpdatedAt] DATETIME2 (7) CONSTRAINT [DF_Countries_UpdatedAt] DEFAULT (getutcdate()) NOT NULL
