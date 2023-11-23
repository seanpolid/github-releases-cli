#!/bin/bash

# Build the tool
mvn package

# Set appropriate permissions
sudo chmod +x release.sh

# Copy shell script and jar to bin
mkdir -p /usr/local/java
sudo cp ./target/*.jar /usr/local/java/*.jar
sudo cp release.sh /usr/local/bin
