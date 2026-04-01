(function () {
    const languageStorageKey = "notes-swagger-lang";
    const supportedLanguages = ["ru", "en"];
    const state = {
        language: "ru",
        notes: null
    };

    const textMapRu = {
        "Try it out": "Попробовать",
        "Execute": "Выполнить",
        "Clear": "Очистить",
        "Cancel": "Отмена",
        "Parameters": "Параметры",
        "No parameters": "Параметров нет",
        "Responses": "Ответы",
        "Description": "Описание",
        "Code": "Код",
        "Links": "Ссылки",
        "No links": "Нет ссылок",
        "Response body": "Тело ответа",
        "Response headers": "Заголовки ответа",
        "Request body": "Тело запроса",
        "Media type": "Тип данных",
        "Controls Accept header.": "Управляет заголовком Accept.",
        "Example Value": "Пример значения",
        "Schema": "Схема",
        "Request URL": "URL запроса",
        "Server response": "Ответ сервера",
        "Request duration": "Время запроса",
        "Available authorizations": "Доступные авторизации",
        "Authorize": "Авторизация",
        "Filter by tag": "Фильтр по тегу",
        "Download": "Скачать",
        "Expand all": "Развернуть все",
        "Collapse all": "Свернуть все"
    };

    const textMapEn = {
        "Попробовать": "Try it out",
        "Выполнить": "Execute",
        "Очистить": "Clear",
        "Отмена": "Cancel",
        "Параметры": "Parameters",
        "Параметров нет": "No parameters",
        "Ответы": "Responses",
        "Описание": "Description",
        "Код": "Code",
        "Ссылки": "Links",
        "Нет ссылок": "No links",
        "Тело ответа": "Response body",
        "Заголовки ответа": "Response headers",
        "Тело запроса": "Request body",
        "Тип данных": "Media type",
        "Управляет заголовком Accept.": "Controls Accept header.",
        "Пример значения": "Example Value",
        "Схема": "Schema",
        "URL запроса": "Request URL",
        "Ответ сервера": "Server response",
        "Время запроса": "Request duration",
        "Доступные авторизации": "Available authorizations",
        "Авторизация": "Authorize",
        "Фильтр по тегу": "Filter by tag",
        "Скачать": "Download",
        "Развернуть все": "Expand all",
        "Свернуть все": "Collapse all",
        "Получить все заметки.": "Get all notes.",
        "Получить заметку по идентификатору.": "Get note by id.",
        "Получить историю изменений заметки.": "Get note revision history.",
        "Поиск заметок по заголовку.": "Search notes by title.",
        "Создать новую заметку.": "Create note.",
        "Обновить существующую заметку.": "Update note.",
        "Удалить заметку по идентификатору.": "Delete note by id.",
        "Поля Title и Content не могут быть пустыми.": "Title and Content cannot be empty.",
        "Параметр query обязателен.": "Query parameter is required."
    };

    const uiText = {
        ru: {
            topTitle: "Notes API",
            topSubtitle: "документация на русском",
            panelTitle: "Быстрый визуальный обзор",
            panelDescription:
                "Рабочий backend для заметок: CRUD, поиск по заголовку и история изменений. Ниже можно сразу открыть endpoint со списком заметок.",
            languageLabel: "Язык интерфейса",
            ctaButton: "Показать список заметок",
            ctaHint: "Кнопка развернет GET /api/Notes и прокрутит к нему.",
            recordsTitle: "Что уже лежит в БД",
            recordsLoading: "Загружаем записи...",
            recordsError: "Не удалось загрузить записи.",
            recordsEmpty: "Записей пока нет.",
            totalsTemplate: "Всего: {total} • Активных: {active} • Архивных: {archived}",
            active: "активна",
            archived: "в архиве",
            noTitle: "Без заголовка"
        },
        en: {
            topTitle: "Notes API",
            topSubtitle: "documentation in English",
            panelTitle: "Quick visual overview",
            panelDescription:
                "Production-like backend for notes: CRUD, title search, and revision history. You can jump to notes endpoint right away.",
            languageLabel: "Interface language",
            ctaButton: "Open notes list",
            ctaHint: "This button expands GET /api/Notes and scrolls to it.",
            recordsTitle: "Data already in database",
            recordsLoading: "Loading records...",
            recordsError: "Could not load records.",
            recordsEmpty: "No records yet.",
            totalsTemplate: "Total: {total} • Active: {active} • Archived: {archived}",
            active: "active",
            archived: "archived",
            noTitle: "Untitled"
        }
    };

    function detectLanguage() {
        const queryLanguage = new URLSearchParams(window.location.search).get("lang");
        if (supportedLanguages.includes(queryLanguage)) {
            localStorage.setItem(languageStorageKey, queryLanguage);
            return queryLanguage;
        }

        const storedLanguage = localStorage.getItem(languageStorageKey);
        if (supportedLanguages.includes(storedLanguage)) {
            return storedLanguage;
        }

        return "ru";
    }

    function getDictionary() {
        return state.language === "ru" ? textMapRu : textMapEn;
    }

    function getUiText() {
        return uiText[state.language];
    }

    function replaceTextNodes(root, dictionary) {
        if (!root) {
            return;
        }

        const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT, null);
        const nodes = [];
        while (walker.nextNode()) {
            nodes.push(walker.currentNode);
        }

        for (const node of nodes) {
            const original = node.nodeValue;
            const trimmed = original?.trim();
            if (!trimmed) {
                continue;
            }

            const replacement = dictionary[trimmed];
            if (!replacement) {
                continue;
            }

            node.nodeValue = original.replace(trimmed, replacement);
        }
    }

    function replaceAttributes(root, dictionary) {
        if (!root) {
            return;
        }

        const attrs = ["placeholder", "title", "aria-label", "value"];
        const elements = root.querySelectorAll("*");
        for (const element of elements) {
            for (const attr of attrs) {
                const value = element.getAttribute(attr);
                if (!value) {
                    continue;
                }

                const translated = dictionary[value];
                if (translated) {
                    element.setAttribute(attr, translated);
                }
            }
        }
    }

    function formatTotals(template, values) {
        return template
            .replace("{total}", values.total.toString())
            .replace("{active}", values.active.toString())
            .replace("{archived}", values.archived.toString());
    }

    function ensureVisualPanel() {
        const infoBlock = document.querySelector(".swagger-ui .info");
        if (!infoBlock) {
            return null;
        }

        let panel = document.querySelector(".swagger-hero");
        if (!panel) {
            panel = document.createElement("section");
            panel.className = "swagger-hero";
            infoBlock.insertAdjacentElement("afterend", panel);
        }

        const copy = getUiText();
        panel.innerHTML = `
            <div class="swagger-hero__row">
                <div>
                    <h2 class="swagger-hero__title">${copy.panelTitle}</h2>
                    <p class="swagger-hero__text">${copy.panelDescription}</p>
                    <div class="swagger-cta">
                        <button type="button" class="swagger-cta__button" data-action="focus-notes">${copy.ctaButton}</button>
                        <p class="swagger-cta__hint">${copy.ctaHint}</p>
                    </div>
                </div>
                <div class="swagger-language">
                    <span class="swagger-language__label">${copy.languageLabel}</span>
                    <div class="swagger-language__buttons">
                        <button type="button" class="swagger-language__button ${state.language === "ru" ? "is-active" : ""}" data-lang="ru">RU</button>
                        <button type="button" class="swagger-language__button ${state.language === "en" ? "is-active" : ""}" data-lang="en">EN</button>
                    </div>
                </div>
            </div>
            <div class="swagger-records">
                <h3 class="swagger-records__title">${copy.recordsTitle}</h3>
                <p class="swagger-records__totals" data-role="totals"></p>
                <ul class="swagger-records__list" data-role="list">
                    <li class="swagger-records__item muted">${copy.recordsLoading}</li>
                </ul>
            </div>
        `;

        const buttons = panel.querySelectorAll(".swagger-language__button");
        for (const button of buttons) {
            button.addEventListener("click", () => {
                const lang = button.getAttribute("data-lang");
                if (!lang || !supportedLanguages.includes(lang)) {
                    return;
                }

                state.language = lang;
                localStorage.setItem(languageStorageKey, lang);
                const url = new URL(window.location.href);
                url.searchParams.set("lang", lang);
                window.history.replaceState({}, "", url.toString());

                ensureVisualPanel();
                applyTranslations();
                renderRecords();
            });
        }

        const ctaButton = panel.querySelector("[data-action='focus-notes']");
        if (ctaButton) {
            ctaButton.addEventListener("click", focusNotesOperation);
        }

        return panel;
    }

    function focusNotesOperation() {
        const opblocks = Array.from(document.querySelectorAll(".swagger-ui .opblock"));
        const target = opblocks.find(opblock => {
            const method = opblock.querySelector(".opblock-summary-method")?.textContent?.trim().toUpperCase();
            const path = opblock.querySelector(".opblock-summary-path")?.textContent?.trim();
            return method === "GET" && /^\/api\/notes$/i.test(path || "");
        });

        if (!target) {
            return;
        }

        const summary = target.querySelector(".opblock-summary");
        if (summary && !target.classList.contains("is-open")) {
            summary.click();
        }

        target.scrollIntoView({ behavior: "smooth", block: "start" });
    }

    function updateHeaderCopy() {
        const copy = getUiText();

        const topLabel = document.querySelector(".swagger-ui .topbar .link span");
        if (topLabel) {
            topLabel.textContent = copy.topTitle;
        }

        const infoTitle = document.querySelector(".swagger-ui .info .title");
        if (infoTitle) {
            infoTitle.innerHTML = `${copy.topTitle} <small>${copy.topSubtitle}</small>`;
        }
    }

    async function loadNotes() {
        if (state.notes) {
            return;
        }

        try {
            let response = await fetch("/api/Notes", { method: "GET" });
            if (!response.ok) {
                response = await fetch("/api/notes", { method: "GET" });
            }

            if (!response.ok) {
                throw new Error(`Status ${response.status}`);
            }

            state.notes = await response.json();
        } catch {
            state.notes = [];
            const panel = document.querySelector(".swagger-hero");
            const list = panel?.querySelector("[data-role='list']");
            if (list) {
                list.innerHTML = `<li class="swagger-records__item muted">${getUiText().recordsError}</li>`;
            }
        }
    }

    function renderRecords() {
        const panel = document.querySelector(".swagger-hero");
        if (!panel) {
            return;
        }

        const list = panel.querySelector("[data-role='list']");
        const totals = panel.querySelector("[data-role='totals']");
        if (!list || !totals) {
            return;
        }

        const copy = getUiText();
        const notes = Array.isArray(state.notes) ? state.notes : [];
        const total = notes.length;
        const archived = notes.filter(note => note.isArchived).length;
        const active = total - archived;

        totals.textContent = formatTotals(copy.totalsTemplate, { total, active, archived });

        if (total === 0) {
            list.innerHTML = `<li class="swagger-records__item muted">${copy.recordsEmpty}</li>`;
            return;
        }

        list.innerHTML = notes.slice(0, 10).map((note, index) => {
            const title = (note.title || "").trim() || copy.noTitle;
            const status = note.isArchived ? copy.archived : copy.active;
            return `
                <li class="swagger-records__item">
                    <span class="swagger-records__idx">#${index + 1}</span>
                    <span class="swagger-records__name">${title}</span>
                    <span class="swagger-records__state">${status}</span>
                </li>
            `;
        }).join("");
    }

    function applyTranslations() {
        updateHeaderCopy();
        const dictionary = getDictionary();
        replaceTextNodes(document.body, dictionary);
        replaceAttributes(document.body, dictionary);
    }

    function scheduleTranslations() {
        applyTranslations();
        setTimeout(applyTranslations, 300);
        setTimeout(applyTranslations, 900);
        setTimeout(applyTranslations, 1600);
    }

    function bindInteractionRefresh() {
        const refresh = () => {
            setTimeout(applyTranslations, 120);
        };

        document.addEventListener("click", refresh);
        document.addEventListener("keyup", refresh);
    }

    async function initWhenReady() {
        for (let i = 0; i < 40; i++) {
            const panel = ensureVisualPanel();
            if (panel) {
                await loadNotes();
                renderRecords();
                scheduleTranslations();
                bindInteractionRefresh();
                return;
            }

            await new Promise(resolve => setTimeout(resolve, 150));
        }
    }

    window.addEventListener("load", () => {
        state.language = detectLanguage();
        initWhenReady();
    });
})();
