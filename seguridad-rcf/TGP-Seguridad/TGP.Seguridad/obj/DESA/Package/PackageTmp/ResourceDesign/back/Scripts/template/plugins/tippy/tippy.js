var tippy = function(t) {
    "use strict";
    t = t && t.hasOwnProperty("default") ? t.default : t;

    function e() {
        return (e = Object.assign || function(t) {
            for (var e = 1; e < arguments.length; e++) {
                var n = arguments[e];
                for (var r in n) Object.prototype.hasOwnProperty.call(n, r) && (t[r] = n[r])
            }
            return t
        }).apply(this, arguments)
    }
    var n = {
            passive: !0
        },
        r = "tippy-iOS",
        i = "tippy-popper",
        o = "tippy-tooltip",
        a = "tippy-content",
        p = "tippy-backdrop",
        u = "tippy-arrow",
        s = "tippy-svg-arrow",
        c = "." + i,
        l = "." + o,
        f = "." + a,
        d = "." + u,
        v = "." + s;

    function m(t, e) {
        t.innerHTML = e
    }

    function h(t) {
        return !(!t || !t._tippy || t._tippy.reference !== t)
    }

    function g(t, e) {
        return {}.hasOwnProperty.call(t, e)
    }

    function b(t) {
        return A(t) ? [t] : function(t) {
            return E(t, "NodeList")
        }(t) ? S(t) : Array.isArray(t) ? t : S(document.querySelectorAll(t))
    }

    function y(t, e, n) {
        if (Array.isArray(t)) {
            var r = t[e];
            return null == r ? Array.isArray(n) ? n[e] : n : r
        }
        return t
    }

    function w(t, e) {
        return t && t.modifiers && t.modifiers[e]
    }

    function E(t, e) {
        var n = {}.toString.call(t);
        return 0 === n.indexOf("[object") && n.indexOf(e + "]") > -1
    }

    function A(t) {
        return E(t, "Element")
    }

    function T(t) {
        return E(t, "MouseEvent")
    }

    function x(t, e) {
        return "function" == typeof t ? t.apply(void 0, e) : t
    }

    function C(t, e, n, r) {
        t.filter(function(t) {
            return t.name === e
        })[0][n] = r
    }

    function I() {
        return document.createElement("div")
    }

    function O(t, e) {
        t.forEach(function(t) {
            t && (t.style.transitionDuration = e + "ms")
        })
    }

    function D(t, e) {
        t.forEach(function(t) {
            t && t.setAttribute("data-state", e)
        })
    }

    function L(t, e) {
        return 0 === e ? t : function(r) {
            clearTimeout(n), n = setTimeout(function() {
                t(r)
            }, e)
        };
        var n
    }

    function M(t, e, n) {
        t && t !== e && t.apply(void 0, n)
    }

    function S(t) {
        return [].slice.call(t)
    }

    function k(t, e) {
        for (; t;) {
            if (e(t)) return t;
            t = t.parentElement
        }
        return null
    }

    function P(t, e) {
        return t.indexOf(e) > -1
    }

    function V(t) {
        return t.split(/\s+/).filter(Boolean)
    }

    function B(t, e) {
        return void 0 !== t ? t : e
    }

    function U(t) {
        return [].concat(t)
    }

    function H(t) {
        var e = U(t)[0];
        return e && e.ownerDocument || document
    }

    function N(t, e) {
        -1 === t.indexOf(e) && t.push(e)
    }

    function z(t) {
        return "number" == typeof t ? t : parseFloat(t)
    }

    function R(t, e, n) {
        void 0 === e && (e = 5);
        var r = {
            top: 0,
            right: 0,
            bottom: 0,
            left: 0
        };
        return Object.keys(r).reduce(function(r, i) {
            return r[i] = "number" == typeof e ? e : e[i], t === i && (r[i] = "number" == typeof e ? e + n : e[t] + n), r
        }, r)
    }
    var q = {
            isTouch: !1
        },
        F = 0;

    function j() {
        q.isTouch || (q.isTouch = !0, window.performance && document.addEventListener("mousemove", _))
    }

    function _() {
        var t = performance.now();
        t - F < 20 && (q.isTouch = !1, document.removeEventListener("mousemove", _)), F = t
    }

    function W() {
        var t = document.activeElement;
        if (h(t)) {
            var e = t._tippy;
            t.blur && !e.state.isVisible && t.blur()
        }
    }
    var X = "undefined" != typeof window && "undefined" != typeof document,
        Y = X ? navigator.userAgent : "",
        J = /MSIE |Trident\//.test(Y),
        G = /UCBrowser\//.test(Y),
        K = X && /iPhone|iPad|iPod/.test(navigator.platform);

    function Q(t) {
        var e = t && K && q.isTouch;
        document.body.classList[e ? "add" : "remove"](r)
    }
    var Z = {
            allowHTML: !0,
            animateFill: !1,
            animation: "fade",
            appendTo: function() {
                return document.body
            },
            aria: "describedby",
            arrow: !0,
            boundary: "scrollParent",
            content: "",
            delay: 0,
            distance: 10,
            duration: [300, 250],
            flip: !0,
            flipBehavior: "flip",
            flipOnUpdate: !1,
            followCursor: !1,
            hideOnClick: !0,
            ignoreAttributes: !1,
            inlinePositioning: !1,
            inertia: !1,
            interactive: !1,
            interactiveBorder: 2,
            interactiveDebounce: 0,
            lazy: !0,
            maxWidth: 350,
            multiple: !1,
            offset: 0,
            onAfterUpdate: function() {},
            onBeforeUpdate: function() {},
            onCreate: function() {},
            onDestroy: function() {},
            onHidden: function() {},
            onHide: function() {},
            onMount: function() {},
            onShow: function() {},
            onShown: function() {},
            onTrigger: function() {},
            onUntrigger: function() {},
            placement: "top",
            plugins: [],
            popperOptions: {},
            role: "tooltip",
            showOnCreate: !1,
            sticky: !1,
            theme: "",
            touch: !0,
            trigger: "mouseenter focus",
            triggerTarget: null,
            updateDuration: 0,
            zIndex: 9999
        },
        $ = Object.keys(Z),
        tt = ["arrow", "boundary", "distance", "flip", "flipBehavior", "flipOnUpdate", "offset", "placement", "popperOptions"];

    function et(t) {
        return e({}, t, {}, t.plugins.reduce(function(e, n) {
            var r = n.name,
                i = n.defaultValue;
            return r && (e[r] = void 0 !== t[r] ? t[r] : i), e
        }, {}))
    }

    function nt(t, n) {
        var r = e({}, n, {
            content: x(n.content, [t])
        }, n.ignoreAttributes ? {} : function(t, n) {
            return (n ? Object.keys(et(e({}, Z, {
                plugins: n
            }))) : $).reduce(function(e, n) {
                var r = (t.getAttribute("data-tippy-" + n) || "").trim();
                if (!r) return e;
                if ("content" === n) e[n] = r;
                else try {
                    e[n] = JSON.parse(r)
                } catch (t) {
                    e[n] = r
                }
                return e
            }, {})
        }(t, n.plugins));
        return r.interactive && (r.aria = null), r
    }

    function rt(t) {
        return t.split("-")[0]
    }

    function it(t) {
        t.setAttribute("data-inertia", "")
    }

    function ot(t) {
        t.setAttribute("data-interactive", "")
    }

    function at(t, e) {
        if (A(e.content)) m(t, ""), t.appendChild(e.content);
        else if ("function" != typeof e.content) {
            t[e.allowHTML ? "innerHTML" : "textContent"] = e.content
        }
    }

    function pt(t) {
        return {
            tooltip: t.querySelector(l),
            content: t.querySelector(f),
            arrow: t.querySelector(d) || t.querySelector(v)
        }
    }

    function ut(t) {
        var e = I();
        return !0 === t ? e.className = u : (e.className = s, A(t) ? e.appendChild(t) : m(e, t)), e
    }

    function st(t, e) {
        var n = I();
        n.className = i, n.style.position = "absolute", n.style.top = "0", n.style.left = "0";
        var r = I();
        r.className = o, r.id = "tippy-" + t, r.setAttribute("data-state", "hidden"), r.setAttribute("tabindex", "-1"), ft(r, "add", e.theme);
        var p = I();
        return p.className = a, p.setAttribute("data-state", "hidden"), e.interactive && ot(r), e.arrow && (r.setAttribute("data-arrow", ""), r.appendChild(ut(e.arrow))), e.inertia && it(r), at(p, e), r.appendChild(p), n.appendChild(r), ct(n, e, e), n
    }

    function ct(t, e, n) {
        var r, i = pt(t),
            o = i.tooltip,
            a = i.content,
            p = i.arrow;
        t.style.zIndex = "" + n.zIndex, o.setAttribute("data-animation", n.animation), o.style.maxWidth = "number" == typeof(r = n.maxWidth) ? r + "px" : r, n.role ? o.setAttribute("role", n.role) : o.removeAttribute("role"), e.content !== n.content && at(a, n), !e.arrow && n.arrow ? (o.appendChild(ut(n.arrow)), o.setAttribute("data-arrow", "")) : e.arrow && !n.arrow ? (o.removeChild(p), o.removeAttribute("data-arrow")) : e.arrow !== n.arrow && (o.removeChild(p), o.appendChild(ut(n.arrow))), !e.interactive && n.interactive ? ot(o) : e.interactive && !n.interactive && function(t) {
            t.removeAttribute("data-interactive")
        }(o), !e.inertia && n.inertia ? it(o) : e.inertia && !n.inertia && function(t) {
            t.removeAttribute("data-inertia")
        }(o), e.theme !== n.theme && (ft(o, "remove", e.theme), ft(o, "add", n.theme))
    }

    function lt(t, e, n) {
        var r = G && void 0 !== document.body.style.webkitTransition ? "webkitTransitionEnd" : "transitionend";
        t[e + "EventListener"](r, n)
    }

    function ft(t, e, n) {
        V(n).forEach(function(n) {
            t.classList[e](n + "-theme")
        })
    }
    var dt = 1,
        vt = [],
        mt = [];

    function ht(r, i) {
        var o, a, p, u = et(nt(r, i));
        if (!u.multiple && r._tippy) return null;
        var s, l, f, d, v, m = !1,
            h = !1,
            b = 0,
            E = [],
            A = L(Ot, u.interactiveDebounce),
            I = H(u.triggerTarget || r),
            F = dt++,
            j = st(F, u),
            _ = pt(j),
            W = (v = u.plugins).filter(function(t, e) {
                return v.indexOf(t) === e
            }),
            X = _.tooltip,
            Y = _.content,
            G = [X, Y],
            K = {
                id: F,
                reference: r,
                popper: j,
                popperChildren: _,
                popperInstance: null,
                props: u,
                state: {
                    currentPlacement: null,
                    isEnabled: !0,
                    isVisible: !1,
                    isDestroyed: !1,
                    isMounted: !1,
                    isShown: !1
                },
                plugins: W,
                clearDelayTimeouts: function() {
                    clearTimeout(o), clearTimeout(a), cancelAnimationFrame(p)
                },
                setProps: function(t) {
                    if (K.state.isDestroyed) return;
                    ft("onBeforeUpdate", [K, t]), Ct();
                    var n = K.props,
                        i = nt(r, e({}, K.props, {}, t, {
                            ignoreAttributes: !0
                        }));
                    i.ignoreAttributes = B(t.ignoreAttributes, n.ignoreAttributes), K.props = i, xt(), n.interactiveDebounce !== i.interactiveDebounce && (bt(), A = L(Ot, i.interactiveDebounce));
                    ct(j, n, i), K.popperChildren = pt(j), n.triggerTarget && !i.triggerTarget ? U(n.triggerTarget).forEach(function(t) {
                        t.removeAttribute("aria-expanded")
                    }) : i.triggerTarget && r.removeAttribute("aria-expanded");
                    if (gt(), K.popperInstance)
                        if (tt.some(function(e) {
                                return g(t, e) && t[e] !== n[e]
                            })) {
                            var o = K.popperInstance.reference;
                            K.popperInstance.destroy(), St(), K.popperInstance.reference = o, K.state.isVisible && K.popperInstance.enableEventListeners()
                        } else K.popperInstance.update();
                    ft("onAfterUpdate", [K, t])
                },
                setContent: function(t) {
                    K.setProps({
                        content: t
                    })
                },
                show: function(t) {
                    void 0 === t && (t = y(K.props.duration, 0, Z.duration));
                    var e = K.state.isVisible,
                        n = K.state.isDestroyed,
                        r = !K.state.isEnabled,
                        i = q.isTouch && !K.props.touch;
                    if (e || n || r || i) return;
                    if (at().hasAttribute("disabled")) return;
                    K.popperInstance || St();
                    if (ft("onShow", [K], !1), !1 === K.props.onShow(K)) return;
                    wt(), j.style.visibility = "visible", K.state.isVisible = !0, K.state.isMounted || O(G.concat(j), 0);
                    l = function() {
                            K.state.isVisible && (O([j], K.props.updateDuration), O(G, t), D(G, "visible"), ht(), gt(), N(mt, K), Q(!0), K.state.isMounted = !0, ft("onMount", [K]), function(t, e) {
                                At(t, e)
                            }(t, function() {
                                K.state.isShown = !0, ft("onShown", [K])
                            }))
                        },
                        function() {
                            b = 0;
                            var t, e = K.props.appendTo,
                                n = at();
                            t = K.props.interactive && e === Z.appendTo || "parent" === e ? n.parentNode : x(e, [n]);
                            t.contains(j) || t.appendChild(j);
                            C(K.popperInstance.modifiers, "flip", "enabled", K.props.flip), K.popperInstance.enableEventListeners(), K.popperInstance.update()
                        }()
                },
                hide: function(t) {
                    void 0 === t && (t = y(K.props.duration, 1, Z.duration));
                    var e = !K.state.isVisible && !m,
                        n = K.state.isDestroyed,
                        r = !K.state.isEnabled && !m;
                    if (e || n || r) return;
                    if (ft("onHide", [K], !1), !1 === K.props.onHide(K) && !m) return;
                    Et(), j.style.visibility = "hidden", K.state.isVisible = !1, K.state.isShown = !1, O(G, t), D(G, "hidden"), ht(), gt(),
                        function(t, e) {
                            At(t, function() {
                                !K.state.isVisible && j.parentNode && j.parentNode.contains(j) && e()
                            })
                        }(t, function() {
                            K.popperInstance.disableEventListeners(), K.popperInstance.options.placement = K.props.placement, j.parentNode.removeChild(j), 0 === (mt = mt.filter(function(t) {
                                return t !== K
                            })).length && Q(!1), K.state.isMounted = !1, ft("onHidden", [K])
                        })
                },
                enable: function() {
                    K.state.isEnabled = !0
                },
                disable: function() {
                    K.hide(), K.state.isEnabled = !1
                },
                destroy: function() {
                    if (K.state.isDestroyed) return;
                    m = !0, K.clearDelayTimeouts(), K.hide(0), Ct(), delete r._tippy, K.popperInstance && K.popperInstance.destroy();
                    m = !1, K.state.isDestroyed = !0, ft("onDestroy", [K])
                }
            };
        r._tippy = K, j._tippy = K;
        var $ = W.map(function(t) {
            return t.fn(K)
        });
        return xt(), gt(), u.lazy || St(), ft("onCreate", [K]), u.showOnCreate && Pt(), j.addEventListener("mouseenter", function() {
            K.props.interactive && K.state.isVisible && K.clearDelayTimeouts()
        }), j.addEventListener("mouseleave", function() {
            K.props.interactive && P(K.props.trigger, "mouseenter") && I.addEventListener("mousemove", A)
        }), K;

        function it() {
            var t = K.props.touch;
            return Array.isArray(t) ? t : [t, 0]
        }

        function ot() {
            return "hold" === it()[0]
        }

        function at() {
            return d || r
        }

        function ut(t) {
            return K.state.isMounted && !K.state.isVisible || q.isTouch || s && "focus" === s.type ? 0 : y(K.props.delay, t ? 0 : 1, Z.delay)
        }

        function ft(t, e, n) {
            var r;
            (void 0 === n && (n = !0), $.forEach(function(n) {
                g(n, t) && n[t].apply(n, e)
            }), n) && (r = K.props)[t].apply(r, e)
        }

        function ht() {
            var t = K.props.aria;
            if (t) {
                var e = "aria-" + t,
                    n = X.id;
                U(K.props.triggerTarget || r).forEach(function(t) {
                    var r = t.getAttribute(e);
                    if (K.state.isVisible) t.setAttribute(e, r ? r + " " + n : n);
                    else {
                        var i = r && r.replace(n, "").trim();
                        i ? t.setAttribute(e, i) : t.removeAttribute(e)
                    }
                })
            }
        }

        function gt() {
            U(K.props.triggerTarget || r).forEach(function(t) {
                K.props.interactive ? t.setAttribute("aria-expanded", K.state.isVisible && t === at() ? "true" : "false") : t.removeAttribute("aria-expanded")
            })
        }

        function bt() {
            I.body.removeEventListener("mouseleave", Vt), I.removeEventListener("mousemove", A), vt = vt.filter(function(t) {
                return t !== A
            })
        }

        function yt(t) {
            if (!K.props.interactive || !j.contains(t.target)) {
                if (at().contains(t.target)) {
                    if (q.isTouch) return;
                    if (K.state.isVisible && P(K.props.trigger, "click")) return
                }!0 === K.props.hideOnClick && (K.clearDelayTimeouts(), K.hide(), h = !0, setTimeout(function() {
                    h = !1
                }), K.state.isMounted || Et())
            }
        }

        function wt() {
            I.addEventListener("mousedown", yt, !0)
        }

        function Et() {
            I.removeEventListener("mousedown", yt, !0)
        }

        function At(t, e) {
            function n(t) {
                t.target === X && (lt(X, "remove", n), e())
            }
            if (0 === t) return e();
            lt(X, "remove", f), lt(X, "add", n), f = n
        }

        function Tt(t, e, n) {
            void 0 === n && (n = !1), U(K.props.triggerTarget || r).forEach(function(r) {
                r.addEventListener(t, e, n), E.push({
                    node: r,
                    eventType: t,
                    handler: e,
                    options: n
                })
            })
        }

        function xt() {
            ot() && (Tt("touchstart", It, n), Tt("touchend", Dt, n)), V(K.props.trigger).forEach(function(t) {
                if ("manual" !== t) switch (Tt(t, It), t) {
                    case "mouseenter":
                        Tt("mouseleave", Dt);
                        break;
                    case "focus":
                        Tt(J ? "focusout" : "blur", Lt)
                }
            })
        }

        function Ct() {
            E.forEach(function(t) {
                var e = t.node,
                    n = t.eventType,
                    r = t.handler,
                    i = t.options;
                e.removeEventListener(n, r, i)
            }), E = []
        }

        function It(t) {
            if (K.state.isEnabled && !Mt(t) && !h)
                if (s = t, d = t.currentTarget, gt(), !K.state.isVisible && T(t) && vt.forEach(function(e) {
                        return e(t)
                    }), "click" === t.type && !1 !== K.props.hideOnClick && K.state.isVisible) Vt(t);
                else {
                    var e = it(),
                        n = e[0],
                        r = e[1];
                    q.isTouch && "hold" === n && r ? o = setTimeout(function() {
                        Pt(t)
                    }, r) : Pt(t)
                }
        }

        function Ot(t) {
            k(t.target, function(t) {
                return t === r || t === j
            }) || function(t, e) {
                var n = e.clientX,
                    r = e.clientY;
                return t.every(function(t) {
                    var e = t.popperRect,
                        i = t.tooltipRect,
                        o = t.interactiveBorder,
                        a = Math.min(e.top, i.top),
                        p = Math.max(e.right, i.right),
                        u = Math.max(e.bottom, i.bottom),
                        s = Math.min(e.left, i.left);
                    return a - r > o || r - u > o || s - n > o || n - p > o
                })
            }(S(j.querySelectorAll(c)).concat(j).map(function(t) {
                var e = t._tippy,
                    n = e.popperChildren.tooltip,
                    r = e.props.interactiveBorder;
                return {
                    popperRect: t.getBoundingClientRect(),
                    tooltipRect: n.getBoundingClientRect(),
                    interactiveBorder: r
                }
            }), t) && (bt(), Vt(t))
        }

        function Dt(t) {
            if (!Mt(t)) return K.props.interactive ? (I.body.addEventListener("mouseleave", Vt), I.addEventListener("mousemove", A), void N(vt, A)) : void Vt(t)
        }

        function Lt(t) {
            t.target === at() && (K.props.interactive && t.relatedTarget && j.contains(t.relatedTarget) || Vt(t))
        }

        function Mt(t) {
            var e = "ontouchstart" in window,
                n = P(t.type, "touch"),
                r = ot();
            return e && q.isTouch && r && !n || q.isTouch && !r && n
        }

        function St() {
            var n, i = K.props.popperOptions,
                o = K.popperChildren.arrow,
                a = w(i, "flip"),
                p = w(i, "preventOverflow");

            function u(t) {
                var e = K.state.currentPlacement;
                K.state.currentPlacement = t.placement, K.props.flip && !K.props.flipOnUpdate && (t.flipped && (K.popperInstance.options.placement = t.placement), C(K.popperInstance.modifiers, "flip", "enabled", !1)), X.setAttribute("data-placement", t.placement), !1 !== t.attributes["x-out-of-boundaries"] ? X.setAttribute("data-out-of-boundaries", "") : X.removeAttribute("data-out-of-boundaries");
                var r = rt(t.placement),
                    i = P(["top", "bottom"], r),
                    o = P(["bottom", "right"], r);
                X.style.top = "0", X.style.left = "0", X.style[i ? "top" : "left"] = (o ? 1 : -1) * n + "px", e && e !== t.placement && K.popperInstance.update()
            }
            var s = e({
                eventsEnabled: !1,
                placement: K.props.placement
            }, i, {
                modifiers: e({}, i && i.modifiers, {
                    tippyDistance: {
                        enabled: !0,
                        order: 0,
                        fn: function(t) {
                            n = function(t, e) {
                                var n = "string" == typeof e && P(e, "rem"),
                                    r = t.documentElement;
                                return r && n ? parseFloat(getComputedStyle(r).fontSize || String(16)) * z(e) : z(e)
                            }(I, K.props.distance);
                            var e = rt(t.placement),
                                r = R(e, p && p.padding, n),
                                i = R(e, a && a.padding, n),
                                o = K.popperInstance.modifiers;
                            return C(o, "preventOverflow", "padding", r), C(o, "flip", "padding", i), t
                        }
                    },
                    preventOverflow: e({
                        boundariesElement: K.props.boundary
                    }, p),
                    flip: e({
                        enabled: K.props.flip,
                        behavior: K.props.flipBehavior
                    }, a),
                    arrow: e({
                        element: o,
                        enabled: !!o
                    }, w(i, "arrow")),
                    offset: e({
                        offset: K.props.offset
                    }, w(i, "offset"))
                }),
                onCreate: function(t) {
                    u(t), M(i && i.onCreate, s.onCreate, [t]), kt()
                },
                onUpdate: function(t) {
                    u(t), M(i && i.onUpdate, s.onUpdate, [t]), kt()
                }
            });
            K.popperInstance = new t(r, j, s)
        }

        function kt() {
            0 === b ? (b++, K.popperInstance.update()) : l && 1 === b && (b++, j.offsetHeight, l())
        }

        function Pt(t) {
            K.clearDelayTimeouts(), K.popperInstance || St(), t && ft("onTrigger", [K, t]), wt();
            var e = ut(!0);
            e ? o = setTimeout(function() {
                K.show()
            }, e) : K.show()
        }

        function Vt(t) {
            if (K.clearDelayTimeouts(), ft("onUntrigger", [K, t]), K.state.isVisible) {
                var e = ut(!1);
                e ? a = setTimeout(function() {
                    K.state.isVisible && K.hide()
                }, e) : p = requestAnimationFrame(function() {
                    K.hide()
                })
            } else Et()
        }
    }

    function gt(t, r, i) {
        void 0 === r && (r = {}), void 0 === i && (i = []), i = Z.plugins.concat(r.plugins || i), document.addEventListener("touchstart", j, e({}, n, {
            capture: !0
        })), window.addEventListener("blur", W);
        var o = e({}, Z, {}, r, {
                plugins: i
            }),
            a = b(t).reduce(function(t, e) {
                var n = e && ht(e, o);
                return n && t.push(n), t
            }, []);
        return A(t) ? a[0] : a
    }
    gt.version = "5.1.2", gt.defaultProps = Z, gt.setDefaultProps = function(t) {
        Object.keys(t).forEach(function(e) {
            Z[e] = t[e]
        })
    }, gt.currentInput = q;
    var bt = {
        mouseover: "mouseenter",
        focusin: "focus",
        click: "click"
    };
    var yt = {
        name: "animateFill",
        defaultValue: !1,
        fn: function(t) {
            var e = t.popperChildren,
                n = e.tooltip,
                r = e.content,
                i = t.props.animateFill && !G ? function() {
                    var t = I();
                    return t.className = p, D([t], "hidden"), t
                }() : null;

            function o() {
                t.popperChildren.backdrop = i
            }
            return {
                onCreate: function() {
                    i && (o(), n.insertBefore(i, n.firstElementChild), n.setAttribute("data-animatefill", ""), n.style.overflow = "hidden", t.setProps({
                        animation: "shift-away",
                        arrow: !1
                    }))
                },
                onMount: function() {
                    if (i) {
                        var t = n.style.transitionDuration,
                            e = Number(t.replace("ms", ""));
                        r.style.transitionDelay = Math.round(e / 10) + "ms", i.style.transitionDuration = t, D([i], "visible")
                    }
                },
                onShow: function() {
                    i && (i.style.transitionDuration = "0ms")
                },
                onHide: function() {
                    i && D([i], "hidden")
                },
                onAfterUpdate: function() {
                    o()
                }
            }
        }
    };
    var wt = {
        name: "followCursor",
        defaultValue: !1,
        fn: function(t) {
            var e, n = t.reference,
                r = t.popper,
                i = H(t.props.triggerTarget || n),
                o = null,
                a = !1,
                p = t.props;

            function u() {
                return "manual" === t.props.trigger.trim()
            }

            function s() {
                var e = !!u() || null !== o && !(0 === o.clientX && 0 === o.clientY);
                return t.props.followCursor && e
            }

            function c() {
                return q.isTouch || "initial" === t.props.followCursor && t.state.isVisible
            }

            function l() {
                t.popperInstance && (t.popperInstance.reference = n)
            }

            function f() {
                if (s() || t.props.placement !== p.placement) {
                    var e = p.placement,
                        n = e.split("-")[1];
                    a = !0, t.setProps({
                        placement: s() && n ? e.replace(n, "start" === n ? "end" : "start") : e
                    }), a = !1
                }
            }

            function d() {
                t.popperInstance && s() && (c() || !0 !== t.props.followCursor) && t.popperInstance.disableEventListeners()
            }

            function v() {
                s() ? i.addEventListener("mousemove", g) : l()
            }

            function m() {
                s() && g(e)
            }

            function h() {
                i.removeEventListener("mousemove", g)
            }

            function g(i) {
                var o = e = i,
                    a = o.clientX,
                    p = o.clientY;
                if (t.popperInstance && t.state.currentPlacement) {
                    var u = k(i.target, function(t) {
                            return t === n
                        }),
                        s = n.getBoundingClientRect(),
                        l = t.props.followCursor,
                        f = "horizontal" === l,
                        d = "vertical" === l,
                        v = P(["top", "bottom"], rt(t.state.currentPlacement)),
                        m = function(t, e) {
                            var n = e ? t.offsetWidth : t.offsetHeight;
                            return {
                                size: n,
                                x: e ? n : 0,
                                y: e ? 0 : n
                            }
                        }(r, v),
                        g = m.size,
                        b = m.x,
                        y = m.y;
                    !u && t.props.interactive || (t.popperInstance.reference = {
                        referenceNode: n,
                        clientWidth: 0,
                        clientHeight: 0,
                        getBoundingClientRect: function() {
                            return {
                                width: v ? g : 0,
                                height: v ? 0 : g,
                                top: (f ? s.top : p) - y,
                                bottom: (f ? s.bottom : p) + y,
                                left: (d ? s.left : a) - b,
                                right: (d ? s.right : a) + b
                            }
                        }
                    }, t.popperInstance.update()), c() && h()
                }
            }
            return {
                onAfterUpdate: function(t, e) {
                    var n;
                    a || (n = e, Object.keys(n).forEach(function(t) {
                        p[t] = B(n[t], p[t])
                    }), e.placement && f()), e.placement && d(), requestAnimationFrame(m)
                },
                onMount: function() {
                    m(), d()
                },
                onShow: function() {
                    u() && (e = o = {
                        clientX: 0,
                        clientY: 0
                    }, f(), v())
                },
                onTrigger: function(t, n) {
                    o || (T(n) && (o = {
                        clientX: n.clientX,
                        clientY: n.clientY
                    }, e = n), f(), v())
                },
                onUntrigger: function() {
                    t.state.isVisible || (h(), o = null)
                },
                onHidden: function() {
                    h(), l(), o = null
                }
            }
        }
    };
    var Et = {
        name: "inlinePositioning",
        defaultValue: !1,
        fn: function(t) {
            var e = t.reference;

            function n() {
                return !!t.props.inlinePositioning
            }
            return {
                onHidden: function() {
                    n() && (t.popperInstance.reference = e)
                },
                onShow: function() {
                    n() && (t.popperInstance.reference = {
                        referenceNode: e,
                        clientWidth: 0,
                        clientHeight: 0,
                        getBoundingClientRect: function() {
                            return function(t, e, n) {
                                if (n.length < 2 || null === t) return e;
                                switch (t) {
                                    case "top":
                                    case "bottom":
                                        var r = n[0],
                                            i = n[n.length - 1],
                                            o = "top" === t,
                                            a = r.top,
                                            p = i.bottom,
                                            u = o ? r.left : i.left,
                                            s = o ? r.right : i.right;
                                        return {
                                            top: a,
                                            bottom: p,
                                            left: u,
                                            right: s,
                                            width: s - u,
                                            height: p - a
                                        };
                                    case "left":
                                    case "right":
                                        var c = Math.min.apply(Math, n.map(function(t) {
                                                return t.left
                                            })),
                                            l = Math.max.apply(Math, n.map(function(t) {
                                                return t.right
                                            })),
                                            f = n.filter(function(e) {
                                                return "left" === t ? e.left === c : e.right === l
                                            }),
                                            d = f[0].top,
                                            v = f[f.length - 1].bottom;
                                        return {
                                            top: d,
                                            bottom: v,
                                            left: c,
                                            right: l,
                                            width: l - c,
                                            height: v - d
                                        };
                                    default:
                                        return e
                                }
                            }(t.state.currentPlacement && rt(t.state.currentPlacement), e.getBoundingClientRect(), S(e.getClientRects()))
                        }
                    })
                }
            }
        }
    };
    var At = {
        name: "sticky",
        defaultValue: !1,
        fn: function(t) {
            var e = t.reference,
                n = t.popper;

            function r(e) {
                return !0 === t.props.sticky || t.props.sticky === e
            }
            var i = null,
                o = null;

            function a() {
                var p = r("reference") ? e.getBoundingClientRect() : null,
                    u = r("popper") ? n.getBoundingClientRect() : null;
                (p && Tt(i, p) || u && Tt(o, u)) && t.popperInstance.update(), i = p, o = u, t.state.isMounted && requestAnimationFrame(a)
            }
            return {
                onMount: function() {
                    t.props.sticky && a()
                }
            }
        }
    };

    function Tt(t, e) {
        return !t || !e || (t.top !== e.top || t.right !== e.right || t.bottom !== e.bottom || t.left !== e.left)
    }
    return X && function(t) {
        var e = document.createElement("style");
        e.textContent = t, e.setAttribute("data-tippy-stylesheet", "");
        var n = document.head,
            r = document.querySelector("head>style,head>link");
        r ? n.insertBefore(e, r) : n.appendChild(e)
    }(".tippy-tooltip[data-animation=fade][data-state=hidden]{opacity:0}.tippy-iOS{cursor:pointer!important;-webkit-tap-highlight-color:transparent}.tippy-popper{pointer-events:none;max-width:calc(100vw - 10px);transition-timing-function:cubic-bezier(.165,.84,.44,1);transition-property:transform}.tippy-tooltip{position:relative;color:#fff;border-radius:4px;font-size:14px;line-height:1.4;background-color:#333;transition-property:visibility,opacity,transform;outline:0}.tippy-tooltip[data-placement^=top]>.tippy-arrow{border-width:8px 8px 0;border-top-color:#333;margin:0 3px;transform-origin:50% 0;bottom:-7px}.tippy-tooltip[data-placement^=bottom]>.tippy-arrow{border-width:0 8px 8px;border-bottom-color:#333;margin:0 3px;transform-origin:50% 7px;top:-7px}.tippy-tooltip[data-placement^=left]>.tippy-arrow{border-width:8px 0 8px 8px;border-left-color:#333;margin:3px 0;transform-origin:0 50%;right:-7px}.tippy-tooltip[data-placement^=right]>.tippy-arrow{border-width:8px 8px 8px 0;border-right-color:#333;margin:3px 0;transform-origin:7px 50%;left:-7px}.tippy-tooltip[data-interactive][data-state=visible]{pointer-events:auto}.tippy-tooltip[data-inertia][data-state=visible]{transition-timing-function:cubic-bezier(.54,1.5,.38,1.11)}.tippy-arrow{position:absolute;border-color:transparent;border-style:solid}.tippy-content{padding:5px 9px}"), gt.setDefaultProps({
        plugins: [yt, wt, Et, At]
    }), gt.createSingleton = function(t, n, r) {
        void 0 === n && (n = {}), void 0 === r && (r = []), r = n.plugins || r, t.forEach(function(t) {
            t.disable()
        });
        var i, o, a = e({}, Z, {}, n).aria,
            p = !1,
            u = t.map(function(t) {
                return t.reference
            }),
            s = {
                fn: function(e) {
                    function n(t) {
                        if (i) {
                            var n = "aria-" + i;
                            t && !e.props.interactive ? o.setAttribute(n, e.popperChildren.tooltip.id) : o.removeAttribute(n)
                        }
                    }
                    return {
                        onAfterUpdate: function(t, n) {
                            var r = n.aria;
                            void 0 !== r && r !== a && (p ? (p = !0, e.setProps({
                                aria: null
                            }), p = !1) : a = r)
                        },
                        onDestroy: function() {
                            t.forEach(function(t) {
                                t.enable()
                            })
                        },
                        onMount: function() {
                            n(!0)
                        },
                        onUntrigger: function() {
                            n(!1)
                        },
                        onTrigger: function(r, p) {
                            var s = p.currentTarget,
                                c = u.indexOf(s);
                            o = s, i = a, e.state.isVisible && n(!0), e.popperInstance.reference = s, e.setContent(t[c].props.content)
                        }
                    }
                }
            };
        return gt(I(), e({}, n, {
            plugins: [s].concat(r),
            aria: null,
            triggerTarget: u
        }))
    }, gt.delegate = function(t, n, r) {
        void 0 === r && (r = []), r = n.plugins || r;
        var i, o, a = [],
            p = [],
            u = n.target,
            s = (i = ["target"], o = e({}, n), i.forEach(function(t) {
                delete o[t]
            }), o),
            c = e({}, s, {
                plugins: r,
                trigger: "manual"
            }),
            l = e({}, s, {
                plugins: r,
                showOnCreate: !0
            }),
            f = gt(t, c);

        function d(t) {
            if (t.target) {
                var e = t.target.closest(u);
                if (e)
                    if (P(e.getAttribute("data-tippy-trigger") || n.trigger || Z.trigger, bt[t.type])) {
                        var r = gt(e, l);
                        r && (p = p.concat(r))
                    }
            }
        }

        function v(t, e, n, r) {
            void 0 === r && (r = !1), t.addEventListener(e, n, r), a.push({
                node: t,
                eventType: e,
                handler: n,
                options: r
            })
        }
        return U(f).forEach(function(t) {
            var e = t.destroy;
            t.destroy = function(t) {
                    void 0 === t && (t = !0), t && p.forEach(function(t) {
                        t.destroy()
                    }), p = [], a.forEach(function(t) {
                        var e = t.node,
                            n = t.eventType,
                            r = t.handler,
                            i = t.options;
                        e.removeEventListener(n, r, i)
                    }), a = [], e()
                },
                function(t) {
                    var e = t.reference;
                    v(e, "mouseover", d), v(e, "focusin", d), v(e, "click", d)
                }(t)
        }), f
    }, gt.hideAll = function(t) {
        var e = void 0 === t ? {} : t,
            n = e.exclude,
            r = e.duration;
        mt.forEach(function(t) {
            var e = !1;
            n && (e = h(n) ? t.reference === n : t.popper === n.popper), e || t.hide(r)
        })
    }, gt.roundArrow = '<svg viewBox="0 0 18 7" xmlns="http://www.w3.org/2000/svg"><path d="M0 7s2.021-.015 5.253-4.218C6.584 1.051 7.797.007 9 0c1.203-.007 2.416 1.035 3.761 2.782C16.012 7.005 18 7 18 7H0z"/></svg>', gt
}(Popper);
//# sourceMappingURL=tippy-bundle.iife.min.js.map