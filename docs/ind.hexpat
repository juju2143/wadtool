bitfield OffsetSize {
	size: 13;
	offset: 19;
};

struct FileList {
	char name[8];
	s16 index;
	s16 children;
	OffsetSize size;
};

char name[32] @ 0;
u32 unknown1 @ 32;
u32 filesize @ 36;
u32 unknown2 @ 40;
u32 unknown3 @ 44;
FileList files[filesize/0x10-3] @ 48;
char pad[4] @ filesize-4;
char pad[0x1000] @ filesize;
