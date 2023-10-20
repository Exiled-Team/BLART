#!/bin/bash

echo "Publishing to local docker repository..."
dotnet publish --arch x64 --os linux .

echo "Pushing to Docker Hub..."
docker push xroier/blart:latest