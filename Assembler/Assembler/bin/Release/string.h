#import std.h
#import malloc.h

#macro ds string length_ret pointer_ret {
    #define pointer_ret malloc_system_pointer
    db_s string pointer_ret length_ret
    #sumdef pointer_ret length_ret
    #sumdef malloc_system_pointer length_ret
}

#macro prints pointer length {
    out_addr pointer length
}

#macro print string {
    #map string str
    #fordef strIterator 0 str.length 1
    out str[strIterator]
    #undef str[strIterator]
    #endfor
    #undef str.length
}

#macro getc ptr_ret {
    malloc ptr_ret 1
scan:
    mov b, 0
    in b
    mov a, b
    add a, 255
    jnc scan
    st ptr
    mov a, 0
}

#macro gets ptr_ret {
    #define ptr_ret malloc_system_pointer
}