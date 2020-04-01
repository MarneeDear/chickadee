namespace chickadee.migrations
open SimpleMigrations

[<Migration(02L, "Create received")>]
type CreateReceived() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE received (
    	date_created	TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    	raw_packet	TEXT NOT NULL,
    	packet_type	TEXT NOT NULL,
    	error	TEXT
    );")

  override __.Down() =
    base.Execute(@"DROP TABLE received;")
