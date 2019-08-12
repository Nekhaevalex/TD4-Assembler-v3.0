#import std.h
#import io.h

main:
    swm 0
    #define HELLO_WORLD_STRING_ADDRESS 0x0
    #define str "Hello_World!"
    ds str HELLO_WORLD_STRING_ADDRESS
    out_addr HELLO_WORLD_STRING_ADDRESS str.length
    #undef str
    #undef HELLO_WORLD_STRING_ADDRESS