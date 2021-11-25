#!/bin/bash

set -e

gcc -c -fPIC library.c
gcc -shared -o library.so -fPIC library.o

dotnet build
