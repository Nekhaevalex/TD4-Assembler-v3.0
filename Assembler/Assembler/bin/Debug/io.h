//Initialize I/O pext and store it's address

#macro init_io $v1 {
	#pext io.pext $v1
	#define $vM $v1
}

//Set working address
#macro set_addr val {
	#map val address 11
	swm $vM
	#define iter1upd 1
	#fordef iter1 0 11 1
	mov b, address[iter1]
	#sumdef iter1upd 1
	st iter1upd
	#undef address[iter1]
	#endfor
	#undef iter1upd
}

//Prepare for reading
#macro mode_read {
	swm $vM
	mov b, 0
	st 13
}

//Prepare for writing
#macro mode_write {
	swm $vM
	mov b, 1
	st 13
}

//Enable auto incrementation -- after writing data address will be increamented automaticly by pext
#macro enable_auto_inc {
	swm $vM
	mov b, 1
	st 14
} 

//Disable auto increment
#macro disable_auto_inc {
	swm $vM
	mov b, 0
	st 14
}

//Enable bus
#macro bus $v1 {
	swm $vM
	mov b, $v1
	st 15
}

#macro add_static_device NAME address {
	#define NAME address
}