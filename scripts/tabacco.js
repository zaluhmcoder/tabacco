                var $tabacco = $('#tabacco');
                var $tabElems = $('#tabacco ul.tabs-holder li');
                var $tabSmallContents = $('#tabacco ul.tabs-holder li .preview-content');
                var $tabContentHolder = $('#tabacco .tabs-content-holder');
                var $tabContentBoxes = $('#tabacco .tabs-content-holder ul li.content-box');

                //params
                var tabsHolderWidth = 900;
                var passiveTabWidth = 32;
                var hoverTabWidth = 250;
                var marginRightPreview = 1;

                //calculates
                var elCount = $tabElems.length;
                var tabWidth = tabsHolderWidth / elCount - marginRightPreview;
                var activeTabWidth = tabsHolderWidth - (elCount - 1) * passiveTabWidth;
                var shownTabWidth = tabsHolderWidth - (elCount - 2) * passiveTabWidth - hoverTabWidth;

                $tabacco.width(tabsHolderWidth);
                $tabElems.width(tabWidth);
                $tabSmallContents.width(tabWidth);
                $tabContentBoxes.width(tabsHolderWidth);

                var tabPreviewPosLeft;
                for (var i = 0; i < elCount; i++) {
                    tabPreviewPosLeft = (tabWidth + marginRightPreview) * i;
                    $tabElems.eq(i).css({
                        left: tabPreviewPosLeft,
                    });
                }

                $tabElems.click(function () {
                    var selectedTabIndex = $(this).index();
                    var prevSelectedTabIndex = $('#tabacco ul.tabs-holder li.selected').index();
                    var $prevSelectedTab = $('#tabacco ul.tabs-holder li.selected');
                    var leftPosition;

                    if ($(this).hasClass('selected'))
                        return false;

                    //check any tab clicked before
                    if (!$tabElems.hasClass('selected')) {
                        $tabSmallContents.hide();
                        $tabContentHolder.slideDown(300);
                        $tabElems.not(':eq(' + selectedTabIndex + ')').width(passiveTabWidth).find('.tab-header span').hide();
                    }
                    else {
                        $tabContentBoxes.eq(prevSelectedTabIndex).hide();
                        $tabElems.eq(prevSelectedTabIndex).width(passiveTabWidth).find('.tab-header span').hide();
                        $prevSelectedTab.removeClass('selected');
                    }

                    //set passive tabs left position
                    for (var i = 0; i < elCount; i++) {
                        if (selectedTabIndex < i) {
                            leftPosition = activeTabWidth + passiveTabWidth * (i - 1);
                        }
                        else if (selectedTabIndex == i) {
                            continue;
                        }
                        else {
                            leftPosition = passiveTabWidth * i;
                        }
                        $tabElems.eq(i).css('left', leftPosition);
                    }

                    //animate left selected tab
                    $tabElems.eq(selectedTabIndex).animate({
                        width: activeTabWidth,
                        left: passiveTabWidth * selectedTabIndex,
                    }, 300).removeClass('hovered');

                    $(this).addClass('selected').find('.tab-header span').show();
                    $tabContentBoxes.eq(selectedTabIndex).delay(100).fadeIn('slow');
                });

                //hover tab then show  tab's text
                $tabElems.hover(function () {
                    var activeTab;
                    var activeTabIndex;
                    var hoveredTab;
                    var hoveredTabIndex;
                    var startElIndex;
                    var endElIndex;
                    var addingTabWidth;

                    if ($('#tabacco ul.tabs-holder li.selected').size() > 0 && !$(this).hasClass('selected')) {
                        activeTab = $('#tabacco ul.tabs-holder li.selected');
                        activeTabIndex = activeTab.index();
                        hoveredTab = $(this);
                        hoveredTabIndex = hoveredTab.index();

                        activeTab.width(shownTabWidth);
                        hoveredTab.width(hoverTabWidth);

                        if (hoveredTabIndex > activeTabIndex) {
                            startElIndex = activeTabIndex + 1;
                            endElIndex = hoveredTabIndex;
                            addingTabWidth = shownTabWidth;
                        }
                        else {
                            startElIndex = hoveredTabIndex + 1;
                            endElIndex = activeTabIndex;
                            addingTabWidth = hoverTabWidth;
                        }

                        //set tabs left position
                        for (var i = startElIndex; i <= endElIndex ; i++) {
                            $tabElems.eq(i).css('left', (i - 1) * passiveTabWidth + addingTabWidth);
                        }
                        hoveredTab.addClass('hovered').find('.tab-header span').show();
                    }

                }, function () {
                    if ($('#tabacco ul.tabs-holder li.selected').size() > 0 && !$(this).hasClass('selected')) {
                        activeTab = $('#tabacco ul.tabs-holder li.selected');
                        activeTabIndex = activeTab.index();
                        hoveredTab = $(this);
                        hoveredTabIndex = hoveredTab.index();

                        hoveredTab.removeClass('hovered').find('.tab-header span').hide();
                        activeTab.width(activeTabWidth);
                        hoveredTab.width(passiveTabWidth);

                        //set tabs left position
                        if (hoveredTabIndex > activeTabIndex) {
                            for (var i = activeTabIndex + 1; i <= hoveredTabIndex ; i++) {
                                $tabElems.eq(i).css('left', (i - 1) * passiveTabWidth + activeTabWidth);
                            }
                        }
                        else {
                            for (var i = hoveredTabIndex; i <= activeTabIndex ; i++) {
                                $tabElems.eq(i).css('left', i * passiveTabWidth);
                            }
                        }
                    }
                });