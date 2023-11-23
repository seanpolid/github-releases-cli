#!/bin/bash

# Build the tool
mvn package

# Set appropriate permissions
chmod +x release.sh

# Copy shell script and jar to bin
mkdir -p /usr/local/java
cp ./target/*.jar /usr/local/java/github_releases_cli.jar
cp release.sh /usr/local/bin/release

# Clean target folder
mvn clean
