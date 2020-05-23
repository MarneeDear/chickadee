namespace chickadee.migrations
open SimpleMigrations

[<Migration(03L, "Create transmitted")>]
type CreateTransmitted() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE transmitted (
	    date_created TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    	raw_packet	TEXT NOT NULL,
    	packet_type	TEXT NOT NULL,
    	transmitted	INTEGER NOT NULL DEFAULT 0,
        PRIMARY KEY(date_created)
    );")

  override __.Down() =
    base.Execute(@"DROP TABLE transmitted;")
