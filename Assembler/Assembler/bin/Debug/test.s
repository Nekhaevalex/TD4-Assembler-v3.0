#import std.h

main:
    swm 0
    #define HELLO_WORLD_STRING_ADDRESS 0x0
    #define str "Hello_World!"
    db_s str HELLO_WORLD_STRING_ADDRESS
    out_addr HELLO_WORLD_STRING_ADDRESS str.length
    #undef str
    #undef HELLO_WORLD_STRING_ADDRESS
    mov b, 10
    out b
    hlt