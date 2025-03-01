# Dev Notes
Updating will be handled in "ticks" which will be run in 8 threads by default for different ticking speeds
(listed in the table below). Ticking methods can be non-public.

Do **NOT** run the tick method inside the render method, this can cause race conditions (two threads modifying at 
once) and slowdowns.

The `TickThread(int thread)` attribute[^1] will allow the program to use a specific thread for it's ticking function, 
this attribute will only work with the object's ticking method (`Tick()`).

| Threads | Ticks per second |
|---------|------------------|
| 0 and 1 | 10               |
| 2 and 3 | 20               |
| 4 and 5 | 30               |
| 6 and 7 | 60               |

Object System:

All game class instances must be registered with the engine, which is automatically done by `GameObject.Create()`

[^1]: Not implemented yet.