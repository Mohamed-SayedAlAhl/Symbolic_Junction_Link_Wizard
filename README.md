# Symbolic Link Creator

## Introduction

The **Symbolic Link Creator** is a C# Windows Forms application designed to create symbolic links and junctions between folders. It allows users to select source and destination directories and manage scheduled paths for creation upon system reboot. This tool is particularly useful for users who need to create shortcuts to folders without duplicating data on their systems.

## Features

- **User-Friendly Interface**: Simple navigation to browse for source and destination directories.
- **Symbolic Links & Junctions**: Options to create both symbolic links and junctions.
- **Scheduled Creation**: Ability to schedule link creation for system reboot.
- **Error Handling**: Handles access denied errors and other exceptions gracefully.
- **Log Management**: Logs actions taken by the application for future reference.

## Usage

1. **Browse for Source and Destination**: Click on the "Browse" buttons to select the desired directories.
2. **Select Link Type**: Choose whether to create a symbolic link or junction using the checkbox.
3. **Create Link**: Click the "Create Symlink" button to create the link.
4. **Scheduled Paths**: The application can schedule link creation upon reboot if necessary.
