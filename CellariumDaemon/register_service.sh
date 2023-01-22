#!/bin/bash

if [ -d "/etc/systemd/system" ]; then
  mv ./cellarium_daemon.service /etc/systemd/system
else
  echo "Directory /etc/systemd/system does not exist. Can\'t register a cellarium daemon service."
fi