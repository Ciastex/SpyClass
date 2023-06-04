let prevObject = null;

function updateClassPathWith(element) {
    if (typeof (element) !== "object") {
        return;
    }

    let segments = [];

    while ($(element).length > 0) {
        segments.push($(element).children("span").eq(0).text());
        element = $(element).parent();
    }

    let pathString = segments.reverse().filter(x => x.length !== 0).join("/");
    
    let docTitleElement = $("#current-document");
    $(docTitleElement).text(pathString);
    $(docTitleElement).attr("title", pathString);

}

function removeOldObjectFocusState() {
    if (prevObject != null) {
        $(prevObject).removeClass("focus");
    }
}

function updateFocusStateOf(element) {
    removeOldObjectFocusState();
    $(element).addClass("focus");
}

function focusTreeMemberByCurrentHash() {
    let href = location.href;

    if (!href.includes("#")) {
        return;
    }

    let target = href.split("#");

    if (target.length > 1) {
        let element = $("li#" + target[1]);

        if (element.length === 0) {
            throw new Error("target doc element not found");
        }

        $(element).trigger("click");
        
        updateFocusStateOf(element);
        updateClassPathWith(element);

        prevObject = element;

        while (element.length > 0) {
            if ($(element).attr("aria-expanded") != null) {
                $(element).attr("aria-expanded", "true");
            }

            element = $(element).parent();
        }
    }
}

window.addEventListener('load', function () {
    $("#left-col").resizable({
        handles: "e"
    });
    
    $("li[doc-file]").each(function () {
        $(this).on("click", null, $(this).attr("id"),
            function (ev) {
                location.hash = ev.data;
                ev.stopPropagation();
            }
        );
    });

    focusTreeMemberByCurrentHash();

    $(window).on("hashchange", function (_) {
        removeOldObjectFocusState();
        updateClassPathWith(this);
        
        $("#main-doc-content").load("types/" + location.hash.replace("#", "") + ".html");
        focusTreeMemberByCurrentHash();
    });
});