(function () {
    "use strict";

    const STORAGE_KEY = "sr_lang";

    const pairs = [
        ["Rent a Vehicle", "Thuê xe"],
        ["Station Map", "Bản đồ trạm"],
        ["My Trip", "Chuyến đi của tôi"],
        ["Logout", "Đăng xuất"],
        ["Profile", "Thông tin cá nhân"],

        ["Fleet Overview", "Tổng quan phương tiện"],
        ["Start Your Rental", "Bắt đầu thuê xe"],
        ["Choose a pick-up station and select a vehicle to start your journey", "Chọn trạm nhận xe và phương tiện để bắt đầu chuyến đi"],
        ["Pick-up Station", "Trạm nhận xe"],
        ["All Stations", "Tất cả trạm"],
        ["All Categories", "Tất cả loại xe"],
        ["🚲 All Categories", "🚲 Tất cả loại xe"],
        ["Available Vehicles at", "Xe có sẵn tại"],
        ["Rent This", "Thuê xe"],
        ["Confirm Rental", "Xác nhận thuê xe"],
        ["Start rental for", "Bắt đầu thuê"],
        ["at", "tại"],
        ["Cancel", "Hủy"],
        ["Confirm", "Xác nhận"],
        ["Available", "Có sẵn"],
        ["Maintenance", "Bảo trì"],
        ["InTransit", "Đang thuê"],
        ["In Transit", "Đang thuê"],
        ["ID:", "Mã:"],

        ["Trip In Progress", "Chuyến đi đang diễn ra"],
        ["Rental ID", "Mã thuê xe"],
        ["Vehicle", "Phương tiện"],
        ["Rate", "Đơn giá"],
        ["Live Duration", "Thời lượng hiện tại"],
        ["Counting up...", "Đang tính thời gian..."],
        ["● Counting up...", "● Đang tính thời gian..."],
        ["Estimated Fare So Far", "Tạm tính hiện tại"],
        ["Calculated by seconds", "Tính theo từng giây"],
        ["● Calculated by seconds", "● Tính theo từng giây"],
        ["Trip Progress", "Tiến trình chuyến đi"],
        ["Start at", "Bắt đầu lúc"],
        ["End destination", "Điểm trả xe"],
        ["End", "Kết thúc"],
        ["Ready to return your vehicle?", "Sẵn sàng trả xe?"],
        ["Select a return station to finish your trip.", "Chọn trạm trả xe để kết thúc chuyến đi."],
        ["Select return station...", "Chọn trạm trả xe..."],
        ["Fare", "Cước phí"],
        ["Discount", "Giảm giá"],
        ["Your Pay", "Bạn thanh toán"],
        ["Select a station to see capacity and discount info", "Chọn trạm để xem sức chứa và thông tin giảm giá"],
        ["15% discount is available only at low inventory stations", "Giảm giá 15% chỉ áp dụng tại trạm thiếu xe"],
        ["End Rental & Checkout", "Kết thúc thuê xe & Thanh toán"],
        ["Return to a low inventory station to get 15% discount!", "Trả xe tại trạm thiếu xe để nhận giảm giá 15%!"],
        ["Find Return Station", "Tìm trạm trả xe"],
        ["Choose a low inventory station to receive 15% fare discount", "Chọn trạm thiếu xe để nhận giảm giá 15%"],
        ["Return to a low-inventory station (<20% capacity) to save on your fare!", "Trả xe tại trạm thiếu xe (<20% sức chứa) để tiết kiệm chi phí!"],
        ["Save 15%", "Tiết kiệm 15%"],
        ["Low Inventory", "Thiếu xe"],
        ["Available Slots", "Chỗ trống"],
        ["Return here → Save 15%", "Trả tại đây → Giảm 15%"],
        ["Full", "Đầy"],
        ["Select", "Chọn"],
        ["Need Help?", "Cần hỗ trợ?"],
        ["Contact our support 24/7", "Liên hệ hỗ trợ 24/7"],
        ["Safe & Secure", "An toàn & bảo mật"],
        ["Your vehicle is secured", "Phương tiện của bạn được bảo vệ"],

        ["Checkout", "Thanh toán"],
        ["Complete Trip & Checkout", "Hoàn tất chuyến đi & Thanh toán"],
        ["Hoàn tất chuyến đi & Thanh toán", "Complete Trip & Checkout"],
        ["Thank you for using SaigonRide. Please review your trip information and complete payment.", "Cảm ơn bạn đã sử dụng SaigonRide. Vui lòng kiểm tra thông tin chuyến đi và hoàn tất thanh toán."],
        ["Cảm ơn bạn đã sử dụng SaigonRide. Vui lòng kiểm tra thông tin chuyến đi và hoàn tất thanh toán.", "Thank you for using SaigonRide. Please review your trip information and complete payment."],
        ["Trip Summary", "Tóm tắt chuyến đi"],
        ["Tóm tắt chuyến đi", "Trip Summary"],
        ["Vehicle ID:", "Mã xe:"],
        ["Mã xe:", "Vehicle ID:"],
        ["Good condition", "Tình trạng tốt"],
        ["Tình trạng tốt", "Good condition"],
        ["Start station", "Trạm bắt đầu"],
        ["Trạm bắt đầu", "Start station"],
        ["Return station", "Trạm trả xe"],
        ["Trạm trả xe", "Return station"],
        ["Ho Chi Minh City", "TP. Hồ Chí Minh"],
        ["TP. Hồ Chí Minh", "Ho Chi Minh City"],
        ["Low-inventory station discount: 15% off", "Ưu đãi trạm thiếu xe: giảm 15%"],
        ["Ưu đãi trạm thiếu xe: giảm 15%", "Low-inventory station discount: 15% off"],
        ["Duration", "Thời lượng"],
        ["Thời lượng", "Duration"],
        ["Unit price", "Đơn giá"],
        ["Đơn giá", "Unit price"],
        ["VND/min", "VND/phút"],
        ["VND/phút", "VND/min"],
        ["Subtotal", "Tạm tính"],
        ["Tạm tính", "Subtotal"],
        ["Discount 15% - Return at low-inventory station", "Giảm 15% - Trả xe tại trạm thiếu xe"],
        ["Giảm 15% - Trả xe tại trạm thiếu xe", "Discount 15% - Return at low-inventory station"],
        ["No low-inventory station discount applied", "Không áp dụng giảm giá trạm thiếu xe"],
        ["Không áp dụng giảm giá trạm thiếu xe", "No low-inventory station discount applied"],
        ["Total payment", "Tổng thanh toán"],
        ["Tổng thanh toán", "Total payment"],
        ["Choose payment method", "Chọn phương thức thanh toán"],
        ["Chọn phương thức thanh toán", "Choose payment method"],
        ["Local payment", "Thanh toán nội địa"],
        ["Thanh toán nội địa", "Local payment"],
        ["International payment", "Thanh toán quốc tế"],
        ["Thanh toán quốc tế", "International payment"],
        ["SaigonRide Wallet", "Ví SaigonRide"],
        ["Ví SaigonRide", "SaigonRide Wallet"],
        ["Recommended", "Đề xuất"],
        ["Đề xuất", "Recommended"],
        ["Balance:", "Số dư:"],
        ["Số dư:", "Balance:"],
        ["Your wallet balance is insufficient. Please choose another payment method.", "Số dư trong ví không đủ. Vui lòng chọn phương thức thanh toán khác."],
        ["Số dư trong ví không đủ. Vui lòng chọn phương thức thanh toán khác.", "Your wallet balance is insufficient. Please choose another payment method."],
        ["Fast payment using wallet balance", "Thanh toán nhanh bằng số dư ví"],
        ["Thanh toán nhanh bằng số dư ví", "Fast payment using wallet balance"],
        ["Pay with MoMo wallet", "Thanh toán bằng ví MoMo"],
        ["Thanh toán bằng ví MoMo", "Pay with MoMo wallet"],
        ["Pay through VNPAY gateway", "Thanh toán qua cổng VNPAY"],
        ["Thanh toán qua cổng VNPAY", "Pay through VNPAY gateway"],
        ["Cash", "Tiền mặt"],
        ["Tiền mặt", "Cash"],
        ["Pay directly at station", "Thanh toán trực tiếp tại trạm"],
        ["Thanh toán trực tiếp tại trạm", "Pay directly at station"],
        ["Fast payment from your device", "Thanh toán nhanh từ thiết bị của bạn"],
        ["Thanh toán nhanh từ thiết bị của bạn", "Fast payment from your device"],
        ["Pay with PayPal account", "Thanh toán bằng tài khoản PayPal"],
        ["Thanh toán bằng tài khoản PayPal", "Pay with PayPal account"],
        ["Confirm payment", "Xác nhận thanh toán"],
        ["Xác nhận thanh toán", "Confirm payment"],
        ["Eco-friendly choice", "Lựa chọn thân thiện môi trường"],
        ["Lựa chọn thân thiện môi trường", "Eco-friendly choice"],
        ["You helped reduce CO₂ emissions", "Bạn đã góp phần giảm phát thải CO₂"],
        ["Bạn đã góp phần giảm phát thải CO₂", "You helped reduce CO₂ emissions"],
        ["Payment information is protected", "Thông tin thanh toán được bảo vệ"],
        ["Thông tin thanh toán được bảo vệ", "Payment information is protected"],

        ["Checkout successful!", "Thanh toán thành công!"],
        ["Thanh toán thành công!", "Checkout successful!"],
        ["Thank you for using SaigonRide", "Cảm ơn bạn đã sử dụng SaigonRide"],
        ["Cảm ơn bạn đã sử dụng SaigonRide", "Thank you for using SaigonRide"],
        ["Invoice details", "Chi tiết hóa đơn"],
        ["Chi tiết hóa đơn", "Invoice details"],
        ["Rental code", "Mã thuê xe"],
        ["Mã thuê xe", "Rental code"],
        ["Payment", "Thanh toán"],
        ["Thanh toán", "Payment"],
        ["Your journey", "Hành trình của bạn"],
        ["Hành trình của bạn", "Your journey"],
        ["Great! You helped reduce about 0.5kg CO₂ today.", "Tuyệt vời! Bạn đã góp phần giảm khoảng 0.5kg CO₂ hôm nay."],
        ["Tuyệt vời! Bạn đã góp phần giảm khoảng 0.5kg CO₂ hôm nay.", "Great! You helped reduce about 0.5kg CO₂ today."],
        ["How was your trip?", "Bạn đánh giá chuyến đi thế nào?"],
        ["Bạn đánh giá chuyến đi thế nào?", "How was your trip?"],
        ["Your feedback helps SaigonRide improve our service", "Phản hồi của bạn giúp SaigonRide cải thiện dịch vụ"],
        ["Phản hồi của bạn giúp SaigonRide cải thiện dịch vụ", "Your feedback helps SaigonRide improve our service"],
        ["Back to rental page", "Về trang thuê xe"],
        ["Về trang thuê xe", "Back to rental page"],

        ["Account information", "Thông tin tài khoản"],
        ["Thông tin tài khoản", "Account information"],
        ["Profile photo", "Ảnh đại diện"],
        ["Ảnh đại diện", "Profile photo"],
        ["JPG, PNG or WebP — max 2MB", "JPG, PNG hoặc WebP — tối đa 2MB"],
        ["JPG, PNG hoặc WebP — tối đa 2MB", "JPG, PNG or WebP — max 2MB"],
        ["Full name", "Họ và tên"],
        ["Họ và tên", "Full name"],
        ["Login email", "Email đăng nhập"],
        ["Email đăng nhập", "Login email"],
        ["This email is used to log in and cannot be changed.", "Email này được dùng để đăng nhập hệ thống và không thể thay đổi."],
        ["Email này được dùng để đăng nhập hệ thống và không thể thay đổi.", "This email is used to log in and cannot be changed."],
        ["User role", "Vai trò người dùng"],
        ["Vai trò người dùng", "User role"],
        ["Passport", "Hộ chiếu"],
        ["Hộ chiếu", "Passport"],
        ["Wallet balance", "Số dư ví"],
        ["Số dư ví", "Wallet balance"],
        ["Save information", "Lưu thông tin"],
        ["Lưu thông tin", "Save information"],
        ["Back to rental", "Quay lại thuê xe"],
        ["Quay lại thuê xe", "Back to rental"],
        ["Current balance", "Số dư hiện tại"],
        ["Số dư hiện tại", "Current balance"],
        ["Top-up amount", "Số tiền nạp"],
        ["Số tiền nạp", "Top-up amount"],
        ["Top up", "Nạp tiền"],
        ["Nạp tiền", "Top up"],
        ["Local user", "Người dùng nội địa"],
        ["Người dùng nội địa", "Local user"],
        ["Foreign tourist", "Khách du lịch nước ngoài"],
        ["Khách du lịch nước ngoài", "Foreign tourist"],
        ["Enter passport number", "Nhập số hộ chiếu"],
        ["Nhập số hộ chiếu", "Enter passport number"],
        ["Enter top-up amount", "Nhập số tiền cần nạp"],
        ["Nhập số tiền cần nạp", "Enter top-up amount"],
        ["Amount must be greater than 0. Wallet is only available for local users.", "Số tiền phải lớn hơn 0. Ví chỉ áp dụng cho người dùng nội địa."],
        ["Số tiền phải lớn hơn 0. Ví chỉ áp dụng cho người dùng nội địa.", "Amount must be greater than 0. Wallet is only available for local users."],

        ["Find nearby stations and monitor real-time vehicle availability", "Tìm trạm gần bạn và theo dõi phương tiện theo thời gian thực"],
        ["Tìm trạm gần bạn và theo dõi phương tiện theo thời gian thực", "Find nearby stations and monitor real-time vehicle availability"],
        ["Low Stock", "Sắp thiếu xe"],
        ["Sắp thiếu xe", "Low Stock"],
        ["Bike", "Xe đạp"],
        ["Xe đạp", "Bike"],
        ["Scooter", "Xe scooter"],
        ["Xe scooter", "Scooter"],
        ["Slots", "Chỗ trống"],
        ["Chỗ trống", "Slots"],

        ["Main Menu", "Menu chính"],
        ["Management", "Quản lý"],
        ["Report", "Báo cáo"],
        ["Dashboard", "Bảng điều khiển"],
        ["Vehicles", "Phương tiện"],
        ["Stations", "Trạm"],
        ["Inventory", "Tồn kho"],
        ["Revenue", "Doanh thu"],
        ["All services online", "Hệ thống hoạt động bình thường"]
    ];

    const toEnglish = Object.create(null);
    const toVietnamese = Object.create(null);

    pairs.forEach(([left, right]) => {
        const a = normalize(left);
        const b = normalize(right);

        if (looksVietnamese(left) && !looksVietnamese(right)) {
            toEnglish[a] = right;
            toVietnamese[b] = left;
        } else {
            toVietnamese[a] = right;
            toEnglish[b] = left;
        }
    });

    function normalize(value) {
        return String(value || "").replace(/\s+/g, " ").trim();
    }

    function looksVietnamese(value) {
        return /[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ]/.test(String(value || ""));
    }

    function translateExact(text, lang) {
        const key = normalize(text);

        if (!key) return text;

        const table = lang === "vi"
            ? toVietnamese
            : toEnglish;

        return table[key] || text;
    }

    function translateWithPatterns(text, lang) {
        const exact = translateExact(text, lang);

        if (exact !== text) return exact;

        const clean = normalize(text);

        if (lang === "vi") {
            if (/^Available Vehicles at\s+(.+)$/i.test(clean)) {
                return clean.replace(
                    /^Available Vehicles at\s+(.+)$/i,
                    "Xe có sẵn tại $1");
            }

            if (/^Start at\s+(.+)$/i.test(clean)) {
                return clean.replace(
                    /^Start at\s+(.+)$/i,
                    "Bắt đầu lúc $1");
            }

            if (/^Return here → Save 15%(.+)$/i.test(clean)) {
                return clean.replace(
                    /^Return here → Save 15%/i,
                    "Trả tại đây → Giảm 15%");
            }

            if (/^Confirm payment\s+(.+)$/i.test(clean)) {
                return clean.replace(
                    /^Confirm payment/i,
                    "Xác nhận thanh toán");
            }

            if (/^Subtotal\s*\((.+)\)$/i.test(clean)) {
                return clean
                    .replace(/^Subtotal/i, "Tạm tính")
                    .replace(/min/g, "phút");
            }

            return clean
                .replace(/\bminutes\b/g, "phút")
                .replace(/\bminute\b/g, "phút")
                .replace(/\bseconds\b/g, "giây")
                .replace(/\bsecond\b/g, "giây")
                .replace(/\bmin\b/g, "phút");
        }

        if (/^Xe có sẵn tại\s+(.+)$/i.test(clean)) {
            return clean.replace(
                /^Xe có sẵn tại\s+(.+)$/i,
                "Available Vehicles at $1");
        }

        if (/^Bắt đầu lúc\s+(.+)$/i.test(clean)) {
            return clean.replace(
                /^Bắt đầu lúc\s+(.+)$/i,
                "Start at $1");
        }

        if (/^Trả tại đây → Giảm 15%(.+)$/i.test(clean)) {
            return clean.replace(
                /^Trả tại đây → Giảm 15%/i,
                "Return here → Save 15%");
        }

        if (/^Xác nhận thanh toán\s+(.+)$/i.test(clean)) {
            return clean.replace(
                /^Xác nhận thanh toán/i,
                "Confirm payment");
        }

        if (/^Tạm tính\s*\((.+)\)$/i.test(clean)) {
            return clean
                .replace(/^Tạm tính/i, "Subtotal")
                .replace(/phút/g, "min");
        }

        return clean
            .replace(/\bphút\b/g, "min")
            .replace(/\bgiây\b/g, "sec");
    }

    function shouldSkipTextNode(node) {
        if (!node || !node.parentElement) return true;

        const tag = node.parentElement.tagName;

        if (["SCRIPT", "STYLE", "NOSCRIPT", "TEXTAREA"].includes(tag)) {
            return true;
        }

        const text = normalize(node.nodeValue);

        if (!text) return true;

        if (/^[\d\s,.:/#%()+\-–—|]+$/.test(text)) return true;

        if (/^[A-Z]{1,4}$/.test(text)) return true;

        if (/^\d+(\.\d+)?\s*(VND|VNĐ|đ|Đ)$/i.test(text)) return true;

        return false;
    }

    function translateTextNode(node, lang) {
        if (shouldSkipTextNode(node)) return;

        const raw = node.nodeValue;
        const leading = raw.match(/^\s*/)[0];
        const trailing = raw.match(/\s*$/)[0];
        const core = normalize(raw);

        const translated = translateWithPatterns(core, lang);

        if (translated !== core) {
            node.nodeValue = leading + translated + trailing;
        }
    }

    function translateAttributes(lang) {
        const attrs = ["placeholder", "title", "aria-label", "alt"];

        document.querySelectorAll("*").forEach(el => {
            attrs.forEach(attr => {
                if (!el.hasAttribute(attr)) return;

                const value = el.getAttribute(attr);
                const translated = translateWithPatterns(value, lang);

                if (translated !== value) {
                    el.setAttribute(attr, translated);
                }
            });
        });
    }

    function translateDataI18n(lang) {
        document.querySelectorAll("[data-i18n]").forEach(el => {
            const key = el.getAttribute("data-i18n");
            const translated = translateWithPatterns(key, lang);

            if (translated !== key) {
                el.textContent = translated;
            }
        });

        document.querySelectorAll("[data-i18n-placeholder]").forEach(el => {
            const key = el.getAttribute("data-i18n-placeholder");
            const translated = translateWithPatterns(key, lang);

            if (translated !== key) {
                el.setAttribute("placeholder", translated);
            }
        });
    }

    function translateTextNodes(lang) {
        if (!document.body) return;

        const walker = document.createTreeWalker(
            document.body,
            NodeFilter.SHOW_TEXT,
            {
                acceptNode(node) {
                    return shouldSkipTextNode(node)
                        ? NodeFilter.FILTER_REJECT
                        : NodeFilter.FILTER_ACCEPT;
                }
            }
        );

        const nodes = [];

        while (walker.nextNode()) {
            nodes.push(walker.currentNode);
        }

        nodes.forEach(node => translateTextNode(node, lang));
    }

    function syncLanguageButtons(lang) {
        document.querySelectorAll("#lang-en, #lang-vi, .lang-btn")
            .forEach(btn => {
                btn.classList.remove("active");
            });

        const activeBtn = document.getElementById("lang-" + lang);

        if (activeBtn) {
            activeBtn.classList.add("active");
        }
    }

    function applyLanguage(lang) {
        const safeLang = lang === "vi"
            ? "vi"
            : "en";

        document.documentElement.lang = safeLang;

        translateDataI18n(safeLang);
        translateAttributes(safeLang);
        translateTextNodes(safeLang);

        localStorage.setItem(STORAGE_KEY, safeLang);

        syncLanguageButtons(safeLang);
    }

    window.setLang = applyLanguage;
    window.setLanguage = applyLanguage;
    window.applyLanguage = applyLanguage;

    document.addEventListener("DOMContentLoaded", () => {
        const currentLang =
            localStorage.getItem(STORAGE_KEY) || "en";

        applyLanguage(currentLang);
    });
})();