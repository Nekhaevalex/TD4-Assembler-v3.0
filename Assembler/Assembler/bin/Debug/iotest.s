#import std.h
#import io.h

main:
    #define tty 1
    set_io_on 2
    bus_on tty
    set_addr 00000000000
    mode_write
    swm 0
    #define INTRO_ADDRESS 0x0
    #define str "Print_your_name:"
    db str INTRO_ADDRESS
    out_addr INTRO_ADDRESS str.length
    mode_read
    out 10
    #define NAME_ADDRESS 0x10
    in_addr NAME_ADDRESS 10
    #define GREET_ADDRESS 0x20
    #define str2 "Hello"
    db str2 GREET_ADDRESS
    mode_write
    out_addr GREET_ADDRESS str2.length
    out_addr NAME_ADDRESS 10
    #undef tty
    #undef INTRO_ADDRESS
    #undef NAME_ADDRESS
    #undef GREET_ADDRESS
    #undef str
    #undef str2