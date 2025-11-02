#!/bin/bash

# Example docker run command with volume mounts to persist settings

docker run -d \
  --name askarr \
  --restart unless-stopped \
  -p 4545:4545 \
  -v $(pwd)/config:/root/config \
  -v $(pwd)/tmp:/root/tmp \
  -e ASPNETCORE_URLS=http://*:4545 \
  amazingmoaaz/askarr:latest

