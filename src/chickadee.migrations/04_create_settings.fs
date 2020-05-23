namespace chickadee.migrations
open SimpleMigrations

[<Migration(04L, "Create settings")>]
type CreateSettings() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE settings (
    	call_sign	TEXT,
    	location_latitude	NUMERIC,
    	location_longitude	NUMERIC,
    	PRIMARY KEY(call_sign)
    );")

  override __.Down() =
    base.Execute(@"DROP TABLE settings;")
