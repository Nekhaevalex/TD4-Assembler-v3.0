//Target: TD4E8

#import std.h

main:
    swm 0
    #define STRING_ADDR 0
    ds "Hello,_World!" STRING_ADDR
    out_addr STRING_ADDR str.length
    out 10
    out 10
    mov a, 0
counter:
    mov b, a
    out b
    mov a, b +1
    jnc counter
    hlt    
    