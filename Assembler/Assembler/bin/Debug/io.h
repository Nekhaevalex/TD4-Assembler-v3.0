#macro set_io_on $v1 {
	#pext io.pext $v1
	#define $vM $v1
}

//Set working address
#macro set_addr address {
	#map address pointers 11
	swm $vM
	#fordef iter 0 11 1
	mov b, pointers[iter]
	st iter
	#endfor
	#fordef iter 0 11 1
	#undef pointers[iter]
	#endfor
	#undef pointers.length
}

//Prepare for reading
#macro mode_read {
	swm $vM
	mov b, 0
	st 12
}

//Prepare for writing
#macro mode_write {
	swm $vM
	mov b, 1
	st 12
}

//Enable auto incrementation -- after writing data address will be increamented automaticly by pext
#macro enable_auto_inc {
	swm $vM
	mov b, 1
	st 13
} 

//Disable auto increment
#macro disable_auto_inc {
	swm $vM
	mov b, 0
	st 13
}

//Enable bus
#macro bus_on $v1 {
	swm $vM
	mov b, 1
	st $v1
}

//Disable bus
#macro bus_off $v1 {
	swm $vM
	mov b, 0
	st $v1
}