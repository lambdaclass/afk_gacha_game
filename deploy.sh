#!/usr/bin/env bash
. "$HOME/.asdf/asdf.sh"
set -ex

if [ -d "/tmp/dark_worlds_server" ]; then
  rm -rf /tmp/dark_worlds_server
fi

cd /tmp/dark_worlds_server
git git@github.com:lambdaclass/dark_worlds_server.git --branch main
cd dark_worlds_server

mix local.hex --force && mix local.rebar --force
mix deps.get --only prod
mix deps.compile
mix assets.deploy
mix compile
mix release
mix phx.gen.release


rm -rf /root/dark_worlds_server
mv /tmp/dark_worlds_server /root/

systemctl stop dark_worlds_server
systemctl daemon-reload
systemctl start dark_worlds_server
