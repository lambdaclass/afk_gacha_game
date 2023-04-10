import Config

# We don't run a server during test. If one is required,
# you can enable the server option below.
config :dark_worlds_server, DarkWorldsServerWeb.Endpoint,
  http: [ip: {127, 0, 0, 1}, port: 4002],
  secret_key_base: "Qu1oU8AKEU4oxh5vqy6daXy8evjMwFiwr52p1MBv2I56bjeyFtCWKyJ3L/9u6NDK",
  server: false

# In test we don't send emails.
config :dark_worlds_server, DarkWorldsServer.Mailer,
  adapter: Swoosh.Adapters.Test

# Disable swoosh api client as it is only required for production adapters.
config :swoosh, :api_client, false

# Print only warnings and errors during test
config :logger, level: :warning

# Initialize plugs at runtime for faster test compilation
config :phoenix, :plug_init_mode, :runtime
