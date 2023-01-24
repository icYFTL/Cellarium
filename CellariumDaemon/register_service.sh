#!/bin/bash

if [ -d "/etc/systemd/system" ]; then
  mv /usr/share/cellarium/daemon/cellarium_daemon.service /etc/systemd/system
  systemctl daemon-reload
else
  echo "Directory /etc/systemd/system does not exist. Can\'t register a cellarium daemon service."
fi