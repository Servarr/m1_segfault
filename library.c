#include <stdio.h>

void print(const char *message)
{
    printf("%s\n", message);
}

int callback(void (*func)(void*,int,const char*))
{
  func(NULL, 0, "Callback hello");
}
