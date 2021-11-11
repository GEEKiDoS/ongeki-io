#include <iostream>
#include <Windows.h>

extern "C"
{
    uint16_t mu3_io_get_api_version(void);
    HRESULT mu3_io_init(void);
    HRESULT mu3_io_poll(void);
    void mu3_io_get_opbtns(uint8_t* opbtn);
    void mu3_io_get_gamebtns(uint8_t* left, uint8_t* right);
    void mu3_io_get_lever(int16_t* pos);
}

int main()
{
	mu3_io_init();

	uint8_t opbtn, leftbtn, rightbtn;
	int16_t lever;

	while(true)
	{
		mu3_io_poll();

		mu3_io_get_opbtns(&opbtn);
		mu3_io_get_lever(&lever);
		mu3_io_get_gamebtns(&leftbtn, &rightbtn);

		printf("%d %d %d %d\n", opbtn, leftbtn, rightbtn, lever);
		Sleep(16);
	}

	return 0;
}
