namespace chickadee.migrations
open SimpleMigrations

[<Migration(03L, "Create transmitted")>]
type CreateTransmitted() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE transmitted (
    	rowid	INTEGER NOT NULL,
    	timestamp	TEXT NOT NULL,
    	message	TEXT NOT NULL,
    	type	TEXT NOT NULL,
    	PRIMARY KEY(rowid)
    );")

  override __.Down() =
    base.Execute(@"DROP TABLE transmitted;")
