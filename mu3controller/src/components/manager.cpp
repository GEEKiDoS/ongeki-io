#include "stdinclude.hpp"

namespace component {
    namespace manager {
        void start() {
            raw_hid::start();
            ongeki_hardware::start();
        }

        void update() {
            raw_hid::update();
        }
    }
}
