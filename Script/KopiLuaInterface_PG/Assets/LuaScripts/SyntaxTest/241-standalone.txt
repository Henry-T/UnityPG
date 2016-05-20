#! /usr/bin/lua
--
-- lua-TestMore : <http://fperrad.github.com/lua-TestMore/>
--
-- Copyright (C) 2009-2011, Perrad Francois
--
-- This code is licensed under the terms of the MIT/X11 license,
-- like Lua itself.
--

--[[

=head1 Lua Stand-alone

=head2 Synopsis

    % perl t/standalone.t

=head2 Description

See "Lua 5.1 Reference Manual", section 6 "Lua Stand-alone",
L<http://www.lua.org/manual/5.1/manual.html#6>.

=cut

--]]

require 'Test.More'

local lua = (platform and platform.lua) or arg[-1]

plan(14)

f = io.open('hello.lua', 'w')
f:write([[
print 'Hello World'
]])
f:close()

cmd = lua .. " hello.lua"
f = io.popen(cmd)
is(f:read'*l', 'Hello World', "file")
f:close()

if arg[-1] == 'luajit' then
    skip("luajit: cannot load Lua bytecode", 1)
else
    os.execute(lua .. "c -o hello.luac hello.lua")

    cmd = lua .. " hello.luac"
    f = io.popen(cmd)
    is(f:read'*l', 'Hello World', "bytecode")
    f:close()

    os.remove('hello.luac') -- clean up
end

cmd = lua .. " < hello.lua"
f = io.popen(cmd)
is(f:read'*l', 'Hello World', "redirect")
f:close()

cmd = lua .. [[ -e"a=1" -e "print(a)"]]
f = io.popen(cmd)
is(f:read'*l', '1', "-e")
f:close()

cmd = lua .. [[ -e"a=1" -e "print(a)" hello.lua]]
f = io.popen(cmd)
is(f:read'*l', '1', "-e & script")
is(f:read'*l', 'Hello World')
f:close()

cmd = lua .. [[ -e "?syntax error?" 2>&1]]
f = io.popen(cmd)
like(f:read'*l', "lua", "-e bad")
f:close()

cmd = lua .. [[ -v 2>&1]]
f = io.popen(cmd)
like(f:read'*l', '^Lua', "-v")
f:close()

cmd = lua .. [[ -v hello.lua 2>&1]]
f = io.popen(cmd)
like(f:read'*l', '^Lua', "-v & script")
is(f:read'*l', 'Hello World')
f:close()

cmd = lua .. [[ -u 2>&1]]
f = io.popen(cmd)
like(f:read'*l', '^usage: ', "unknown option")
f:close()

cmd = lua .. [[ -lTest.More -e "print(type(ok))"]]
f = io.popen(cmd)
is(f:read'*l', 'function', "-lTest.More")
f:close()

cmd = lua .. [[ -l Test.More -e "print(type(ok))"]]
f = io.popen(cmd)
is(f:read'*l', 'function', "-l Test.More")
f:close()

cmd = lua .. [[ -l no_lib hello.lua 2>&1]]
f = io.popen(cmd)
like(f:read'*l', "^[^:]+: module 'no_lib' not found:", "-l no lib")
f:close()

os.remove('hello.lua') -- clean up

-- Local Variables:
--   mode: lua
--   lua-indent-level: 4
--   fill-column: 100
-- End:
-- vim: ft=lua expandtab shiftwidth=4:
